using ERP.Application.Identity.Models;

namespace ERP.Api.Contracts.Authorization;

public sealed record PermissionResponse(Guid Id, string Code, string Description)
{
    public static PermissionResponse FromDto(PermissionDto permission)
    {
        return new PermissionResponse(permission.Id, permission.Code, permission.Description);
    }
}
