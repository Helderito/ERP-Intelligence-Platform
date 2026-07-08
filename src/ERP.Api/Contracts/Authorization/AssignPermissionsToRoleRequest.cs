namespace ERP.Api.Contracts.Authorization;

public sealed record AssignPermissionsToRoleRequest(IReadOnlyCollection<Guid> PermissionIds);
