using ERP.Api.Contracts.Authorization;
using ERP.Application.Identity.Authorization;
using ERP.Application.Identity.Queries;
using ERP.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = PermissionPolicies.RolesManage)]
[Route("permissions")]
public sealed class PermissionsController : ControllerBase
{
    private readonly AuthorizationService _authorizationService;

    public PermissionsController(AuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PermissionResponse>>> GetPermissions(
        CancellationToken cancellationToken)
    {
        var permissions = await _authorizationService.GetPermissionsAsync(new GetPermissionsQuery(), cancellationToken);

        return Ok(permissions.Select(PermissionResponse.FromDto).ToArray());
    }
}
