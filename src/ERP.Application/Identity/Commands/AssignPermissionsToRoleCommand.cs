namespace ERP.Application.Identity.Commands;

public sealed record AssignPermissionsToRoleCommand(Guid RoleId, IReadOnlyCollection<Guid> PermissionIds);
