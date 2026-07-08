using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.Authorization;
using ERP.Application.Identity.Authorization;
using ERP.Application.Identity.Commands;
using ERP.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Authorize(Policy = PermissionPolicies.UsersManage)]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private readonly AuthorizationService _authorizationService;

    public UsersController(AuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssignRoles(
        Guid id,
        AssignRolesToUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _authorizationService.AssignRolesToUserAsync(
                new AssignRolesToUserCommand(id, request.RoleIds),
                cancellationToken);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }
}
