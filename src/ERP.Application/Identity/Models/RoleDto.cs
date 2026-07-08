namespace ERP.Application.Identity.Models;

public sealed record RoleDto(
    Guid Id,
    string Name,
    bool IsActive,
    IReadOnlyCollection<Guid> PermissionIds);
