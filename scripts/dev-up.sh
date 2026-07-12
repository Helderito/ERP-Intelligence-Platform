#!/usr/bin/env bash
#
# Rebuilds and restarts the local ERP stack from the current branch, then waits until the API is healthy.
#
# Run this after every merge or pull so the running containers match the latest code.
# A plain `docker compose up -d` reuses the OLD image; the `--build` here recompiles the
# API (applying any new EF migrations on startup) and the web bundle.
#
# Usage: ./scripts/dev-up.sh
set -euo pipefail

echo "==> Pulling latest (fast-forward only)..."
git pull --ff-only

echo "==> Rebuilding and starting containers..."
docker compose up -d --build

echo "==> Waiting for the API to become healthy..."
# A protected endpoint answering 401 proves the API process is up and serving requests.
ready=0
for _ in $(seq 1 30); do
  code=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/suppliers || echo 000)
  if [ "$code" = "401" ] || [ "$code" = "200" ]; then
    ready=1
    break
  fi
  sleep 2
done

if [ "$ready" = "1" ]; then
  echo ""
  echo "App ready to test:"
  echo "  Web: http://localhost:5173  (log in, then Clientes / Fornecedores / Produtos)"
  echo "  API: http://localhost:5000"
else
  echo ""
  echo "The API did not become healthy in time. Inspect the logs:"
  echo "  docker logs erp-intelligence-platform-api-1 --tail 40"
  exit 1
fi
