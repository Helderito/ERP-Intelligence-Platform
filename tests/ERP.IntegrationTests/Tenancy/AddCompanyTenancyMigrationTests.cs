using ERP.Domain.Tenancy;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Testcontainers.PostgreSql;

namespace ERP.IntegrationTests.Tenancy;

public sealed class AddCompanyTenancyMigrationTests
{
    [Fact]
    public async Task Migration_ShouldBackfillExistingMasterDataAndUserMembershipWithoutDataLoss()
    {
        await using var postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("ERPIntelligenceMigrationTests")
            .WithUsername("erp")
            .WithPassword("erp_test_password")
            .Build();
        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;
        await using var dbContext = new AppDbContext(options, new DefaultCompanyProvider());
        var migrator = dbContext.Database.GetService<IMigrator>();
        await migrator.MigrateAsync("20260912110038_AddSharedReferenceData");

        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var taxCodeId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var createdAtUtc = new DateTime(2026, 9, 12, 12, 0, 0, DateTimeKind.Utc);

        await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO "User" ("Id", "Email", "PasswordHash", "CreatedAtUtc", "IsActive")
            VALUES ({userId}, 'migration-user@example.com', 'hash', {createdAtUtc}, TRUE);
            INSERT INTO "Customer" ("Id", "Code", "Name", "IsActive", "CreatedAtUtc")
            VALUES ({customerId}, 'CUS-MIG', 'Migration Customer', TRUE, {createdAtUtc});
            INSERT INTO "Supplier" ("Id", "Code", "Name", "IsActive", "CreatedAtUtc")
            VALUES ({supplierId}, 'SUP-MIG', 'Migration Supplier', TRUE, {createdAtUtc});
            INSERT INTO "Product" ("Id", "Code", "Name", "CategoryId", "UnitOfMeasureId", "IsActive", "CreatedAtUtc")
            VALUES ({productId}, 'SKU-MIG', 'Migration Product', {MasterDataSeed.GeneralCategoryId},
                    {MasterDataSeed.UnitOfMeasureUnitId}, TRUE, {createdAtUtc});
            INSERT INTO "TaxCode" ("Id", "Code", "Name", "Rate", "IsActive", "CreatedAtUtc")
            VALUES ({taxCodeId}, 'VAT-MIG', 'Migration VAT', 14, TRUE, {createdAtUtc});
            INSERT INTO "Warehouse" ("Id", "Code", "Name", "WarehouseTypeId", "IsActive", "CreatedAtUtc")
            VALUES ({warehouseId}, 'WH-MIG', 'Migration Warehouse', {MasterDataSeed.MainWarehouseTypeId}, TRUE, {createdAtUtc});
            """);

        await migrator.MigrateAsync();
        dbContext.ChangeTracker.Clear();

        Assert.Equal(TenancySeed.DefaultCompanyId, await dbContext.Customers.Select(item => item.CompanyId).SingleAsync());
        Assert.Equal(TenancySeed.DefaultCompanyId, await dbContext.Suppliers.Select(item => item.CompanyId).SingleAsync());
        Assert.Equal(TenancySeed.DefaultCompanyId, await dbContext.Products.Select(item => item.CompanyId).SingleAsync());
        Assert.Equal(TenancySeed.DefaultCompanyId, await dbContext.TaxCodes.Select(item => item.CompanyId).SingleAsync());
        Assert.Equal(TenancySeed.DefaultCompanyId, await dbContext.Warehouses.Select(item => item.CompanyId).SingleAsync());
        Assert.All(await dbContext.Categories.ToArrayAsync(), item => Assert.Equal(TenancySeed.DefaultCompanyId, item.CompanyId));
        Assert.All(await dbContext.UnitsOfMeasure.ToArrayAsync(), item => Assert.Equal(TenancySeed.DefaultCompanyId, item.CompanyId));
        Assert.Equal(
            TenancySeed.DefaultCompanyId,
            await dbContext.UserCompanies.Where(item => item.UserId == userId).Select(item => item.CompanyId).SingleAsync());
    }

    private sealed class DefaultCompanyProvider : ICurrentCompanyProvider
    {
        public Guid? UserId => null;
        public Guid? CompanyId => TenancySeed.DefaultCompanyId;
        public Guid GetRequiredCompanyId() => TenancySeed.DefaultCompanyId;
    }
}
