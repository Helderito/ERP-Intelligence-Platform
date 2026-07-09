using System.Security.Claims;
using ERP.Application.Identity.Queries;
using ERP.Application.Identity.Services;
using Microsoft.AspNetCore.Authorization;

namespace ERP.Infrastructure.Identity.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly AuthorizationService _authorizationService;

    public PermissionAuthorizationHandler(AuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdValue = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return;
        }

        IReadOnlyCollection<string> permissions;

        try
        {
            permissions = await _authorizationService.ListUserPermissionsAsync(
                new ListUserPermissionsQuery(userId));
        }
        catch (InvalidOperationException)
        {
            return;
        }

        if (permissions.Contains(requirement.PermissionCode, StringComparer.Ordinal))
        {
            context.Succeed(requirement);
        }
    }
}
