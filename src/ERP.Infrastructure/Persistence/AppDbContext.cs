using Microsoft.EntityFrameworkCore;
using ERP.Domain.Identity;
using ERP.Domain.MasterData;
using ERP.Domain.Tenancy;

namespace ERP.Infrastructure.Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ICurrentCompanyProvider currentCompanyProvider) : DbContext(options)
{
    private readonly ICurrentCompanyProvider _currentCompanyProvider = currentCompanyProvider;

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();

    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<SupplierContact> SupplierContacts => Set<SupplierContact>();

    public DbSet<SupplierAddress> SupplierAddresses => Set<SupplierAddress>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<WarehouseType> WarehouseTypes => Set<WarehouseType>();

    public DbSet<TaxCode> TaxCodes => Set<TaxCode>();

    public DbSet<Country> Countries => Set<Country>();

    public DbSet<Currency> Currencies => Set<Currency>();

    public DbSet<PaymentTerm> PaymentTerms => Set<PaymentTerm>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Establishment> Establishments => Set<Establishment>();

    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Customer>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
        modelBuilder.Entity<Supplier>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
        modelBuilder.Entity<Product>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
        modelBuilder.Entity<Category>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
        modelBuilder.Entity<UnitOfMeasure>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
        modelBuilder.Entity<TaxCode>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
        modelBuilder.Entity<Warehouse>().HasQueryFilter(entity =>
            entity.CompanyId == _currentCompanyProvider.CompanyId);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ValidateCompanyScope();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyScope();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ValidateCompanyScope()
    {
        var scopedEntries = ChangeTracker.Entries<ICompanyOwned>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        if (scopedEntries.Length == 0)
        {
            return;
        }

        var currentCompanyId = _currentCompanyProvider.GetRequiredCompanyId();
        if (scopedEntries.Any(entry => entry.Entity.CompanyId != currentCompanyId))
        {
            throw new InvalidOperationException("A company-owned entity cannot be changed outside the current company.");
        }
    }
}
