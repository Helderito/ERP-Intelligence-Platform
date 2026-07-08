namespace ERP.Application.Identity.Commands;

public sealed record UpdateRoleCommand(Guid RoleId, string Name);
