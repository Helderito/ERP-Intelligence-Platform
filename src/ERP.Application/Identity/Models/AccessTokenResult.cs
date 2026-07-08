namespace ERP.Application.Identity.Models;

public sealed record AccessTokenResult(string AccessToken, DateTime ExpiresAtUtc);
