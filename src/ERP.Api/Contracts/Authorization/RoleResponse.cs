using ERP.Application.Identity.Models;

namespace ERP.Api.Contracts.Authorization;

public sealed record RoleResponse(
    Guid Id,
    string Name,
    bool IsActive,
    IReadOnlyCollection<Guid> PermissionIds)
{
    public static RoleResponse FromDto(RoleDto role)
    {
        return new RoleResponse(role.Id, role.Name, role.IsActive, role.PermissionIds);
    }
}
