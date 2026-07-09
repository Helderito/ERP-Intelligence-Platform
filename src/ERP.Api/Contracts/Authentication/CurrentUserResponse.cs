namespace ERP.Api.Contracts.Authentication;

public sealed record CurrentUserResponse(
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
