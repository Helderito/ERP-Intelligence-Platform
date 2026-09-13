namespace ERP.Domain.Tenancy;

public static class TenancySeed
{
    public static readonly Guid DefaultCompanyId = Guid.Parse("5f0a6d53-5e2d-4f65-aeb8-000000000001");

    public static readonly Guid DefaultEstablishmentId = Guid.Parse("5f0a6d53-5e2d-4f65-aeb8-000000000002");

    public static readonly DateTime SeededAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
