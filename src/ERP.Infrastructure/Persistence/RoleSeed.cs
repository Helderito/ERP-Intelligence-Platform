namespace ERP.Infrastructure.Persistence;

/// <summary>
/// Fixed values for the seeded Administrator role and its permission links.
/// Deterministic (no <see cref="System.DateTime.UtcNow"/>) so the seed is
/// stable across migrations, per the HasData requirements.
/// </summary>
public static class RoleSeed
{
    public static readonly DateTime SeededAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid AdministratorRolesManageLinkId = Guid.Parse("b1a7c0de-0000-4000-a000-000000000101");

    public static readonly Guid AdministratorUsersManageLinkId = Guid.Parse("b1a7c0de-0000-4000-a000-000000000102");

    public static readonly Guid AdministratorCatalogManageLinkId = Guid.Parse("b1a7c0de-0000-4000-a000-000000000103");

    public static readonly Guid AdministratorCustomersManageLinkId = Guid.Parse("b1a7c0de-0000-4000-a000-000000000104");
}
