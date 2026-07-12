<#
.SYNOPSIS
    Rebuilds and restarts the local ERP stack from the current branch, then waits until the API is healthy.

.DESCRIPTION
    Run this after every merge or pull so the running containers match the latest code.
    A plain `docker compose up -d` reuses the OLD image; the `--build` here recompiles the
    API (applying any new EF migrations on startup) and the web bundle. Compatible with
    Windows PowerShell 5.1 and PowerShell 7+.

.EXAMPLE
    ./scripts/dev-up.ps1
#>
$ErrorActionPreference = "Stop"

Write-Host "==> Pulling latest (fast-forward only)..." -ForegroundColor Cyan
git pull --ff-only

Write-Host "==> Rebuilding and starting containers..." -ForegroundColor Cyan
docker compose up -d --build

Write-Host "==> Waiting for the API to become healthy..." -ForegroundColor Cyan
# A protected endpoint answering 401 proves the API process is up and serving requests.
$apiUrl = "http://localhost:5000/suppliers"
$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    try {
        Invoke-WebRequest -Uri $apiUrl -Method GET -TimeoutSec 3 -UseBasicParsing | Out-Null
        $ready = $true; break   # 200 OK
    } catch {
        $response = $_.Exception.Response
        if ($response -and [int]$response.StatusCode -ge 400 -and [int]$response.StatusCode -lt 500) {
            $ready = $true; break   # 401/403 also means the API is answering
        }
    }
    Start-Sleep -Seconds 2
}

if ($ready) {
    Write-Host "`nApp ready to test:" -ForegroundColor Green
    Write-Host "  Web: http://localhost:5173  (log in, then Clientes / Fornecedores / Produtos)"
    Write-Host "  API: http://localhost:5000"
} else {
    Write-Host "`nThe API did not become healthy in time. Inspect the logs:" -ForegroundColor Red
    Write-Host "  docker logs erp-intelligence-platform-api-1 --tail 40"
    exit 1
}
