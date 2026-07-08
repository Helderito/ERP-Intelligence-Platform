namespace ERP.Application.Identity.Models;

public sealed record AuthenticationResult(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
