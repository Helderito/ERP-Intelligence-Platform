using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.Authorization;
using ERP.Application.Identity.Authorization;
using ERP.Application.Identity.Commands;
using ERP.Application.Identity.Queries;
using ERP.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = PermissionPolicies.RolesManage)]
[Route("roles")]
public sealed class RolesController : ControllerBase
{
    private readonly AuthorizationService _authorizationService;

    public RolesController(AuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RoleResponse>>> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await _authorizationService.GetRolesAsync(new GetRolesQuery(), cancellationToken);

        return Ok(roles.Select(RoleResponse.FromDto).ToArray());
    }

    [HttpPost]
    public async Task<ActionResult<RoleResponse>> CreateRole(
        CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var role = await _authorizationService.CreateRoleAsync(
                new CreateRoleCommand(request.Name),
                cancellationToken);

            return Created($"/roles/{role.Id}", RoleResponse.FromDto(role));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoleResponse>> UpdateRole(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var role = await _authorizationService.UpdateRoleAsync(
                new UpdateRoleCommand(id, request.Name),
                cancellationToken);

            return Ok(RoleResponse.FromDto(role));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateRole(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _authorizationService.DeactivateRoleAsync(
                new DeactivateRoleCommand(id),
                cancellationToken);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse(ex.Message));
        }
    }

    [HttpPost("{id:guid}/permissions")]
    public async Task<ActionResult<RoleResponse>> AssignPermissions(
        Guid id,
        AssignPermissionsToRoleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var role = await _authorizationService.AssignPermissionsToRoleAsync(
                new AssignPermissionsToRoleCommand(id, request.PermissionIds),
                cancellationToken);

            return Ok(RoleResponse.FromDto(role));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }
}
