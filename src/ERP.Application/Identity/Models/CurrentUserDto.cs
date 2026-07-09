namespace ERP.Application.Identity.Models;

public sealed record CurrentUserDto(
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
