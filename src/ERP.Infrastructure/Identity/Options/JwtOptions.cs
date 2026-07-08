namespace ERP.Infrastructure.Identity.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "ERP Intelligence Platform";

    public string Audience { get; init; } = "ERP Intelligence Platform";

    public string SigningKey { get; init; } = string.Empty;

    public int AccessTokenMinutes { get; init; } = 15;
}
