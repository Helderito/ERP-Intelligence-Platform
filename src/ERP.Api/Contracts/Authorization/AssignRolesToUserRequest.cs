namespace ERP.Api.Contracts.Authorization;

public sealed record AssignRolesToUserRequest(IReadOnlyCollection<Guid> RoleIds);
