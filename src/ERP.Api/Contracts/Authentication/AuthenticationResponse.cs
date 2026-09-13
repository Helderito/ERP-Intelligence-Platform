using ERP.Application.Identity.Models;

namespace ERP.Api.Contracts.Authentication;

public sealed record AuthenticationResponse(
    Guid UserId,
    Guid CompanyId,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions)
{
    public static AuthenticationResponse FromResult(AuthenticationResult result)
    {
        return new AuthenticationResponse(
            result.UserId,
            result.CompanyId,
            result.Email,
            result.AccessToken,
            result.AccessTokenExpiresAtUtc,
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc,
            result.Roles,
            result.Permissions);
    }
}
