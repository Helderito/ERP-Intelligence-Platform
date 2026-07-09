namespace ERP.Application.Identity.Commands;

public sealed record AssignRolesToUserCommand(Guid UserId, IReadOnlyCollection<Guid> RoleIds);
