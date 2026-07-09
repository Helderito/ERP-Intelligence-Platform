namespace ERP.Domain.Identity;

/// <summary>
/// Well-known identifiers for identity data seeded into the database.
/// Kept in the Domain layer so both the Application layer (first-user
/// bootstrap) and the Infrastructure layer (migration seed) reference a
/// single source of truth.
/// </summary>
public static class IdentitySeed
{
    public static readonly Guid AdministratorRoleId = Guid.Parse("b1a7c0de-0000-4000-a000-000000000001");

    public const string AdministratorRoleName = "Administrator";
}
