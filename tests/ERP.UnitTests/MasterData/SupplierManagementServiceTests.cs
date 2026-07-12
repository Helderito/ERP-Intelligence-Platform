using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class SupplierManagementServiceTests
{
    [Fact]
    public async Task CreateSupplierAsync_ShouldCreateSupplierWithContactsAndAddresses_WhenCodeIsUnique()
    {
        var supplierRepository = new FakeSupplierRepository();
        var service = new SupplierManagementService(supplierRepository);

        var supplier = await service.CreateSupplierAsync(
            new CreateSupplierCommand(
                "sup-001",
                "Sample Supplier",
                [new SupplierContactCommand("Ana Silva", "ana@example.com", "+244 900 000 000")],
                [new SupplierAddressCommand("Rua Principal", null, "Luanda", "1000", "Angola")]));

        Assert.Equal("SUP-001", supplier.Code);
        Assert.Single(supplier.Contacts);
        Assert.Single(supplier.Addresses);
        Assert.Single(supplierRepository.Suppliers);
    }

    [Fact]
    public async Task CreateSupplierAsync_ShouldThrow_WhenCodeAlreadyExists()
    {
        var supplierRepository = new FakeSupplierRepository();
        var service = new SupplierManagementService(supplierRepository);

        await service.CreateSupplierAsync(new CreateSupplierCommand("sup-001", "Sample Supplier", [], []));

        await Assert.ThrowsAsync<SupplierCodeAlreadyExistsException>(
            () => service.CreateSupplierAsync(new CreateSupplierCommand("SUP-001", "Duplicate", [], [])));
    }

    [Fact]
    public async Task UpdateSupplierAsync_ShouldReplaceContactsAndAddresses()
    {
        var supplierRepository = new FakeSupplierRepository();
        var service = new SupplierManagementService(supplierRepository);
        var created = await service.CreateSupplierAsync(
            new CreateSupplierCommand(
                "sup-001",
                "Sample Supplier",
                [new SupplierContactCommand("Ana Silva", "ana@example.com", null)],
                [new SupplierAddressCommand("Rua Principal", null, "Luanda", "1000", "Angola")]));

        var updated = await service.UpdateSupplierAsync(
            new UpdateSupplierCommand(
                created.Id,
                "Updated Supplier",
                [new SupplierContactCommand("Carlos Lopes", "carlos@example.com", null)],
                [new SupplierAddressCommand("Avenida Nova", "2A", "Lisboa", "1200-001", "Portugal")]));

        Assert.Equal("Updated Supplier", updated.Name);
        Assert.Single(updated.Contacts);
        Assert.Equal("Carlos Lopes", updated.Contacts.Single().Name);
        Assert.Single(updated.Addresses);
        Assert.Equal("Lisboa", updated.Addresses.Single().City);
    }

    [Fact]
    public async Task UpdateSupplierAsync_ShouldThrow_WhenSupplierDoesNotExist()
    {
        var service = new SupplierManagementService(new FakeSupplierRepository());

        await Assert.ThrowsAsync<SupplierNotFoundException>(
            () => service.UpdateSupplierAsync(new UpdateSupplierCommand(Guid.NewGuid(), "Missing", [], [])));
    }

    [Fact]
    public async Task SearchSuppliersAsync_ShouldReturnPagedLightResults()
    {
        var supplierRepository = new FakeSupplierRepository();
        var service = new SupplierManagementService(supplierRepository);

        await service.CreateSupplierAsync(new CreateSupplierCommand("sup-001", "Sample Supplier", [], []));

        var result = await service.SearchSuppliersAsync(new SearchSuppliersQuery("sample", 1, 20));

        Assert.Equal(1, result.TotalRecords);
        var supplier = Assert.Single(result.Items);
        Assert.Equal("SUP-001", supplier.Code);
        Assert.Equal("Sample Supplier", supplier.Name);
        Assert.True(supplier.IsActive);
    }

    private sealed class FakeSupplierRepository : ISupplierRepository
    {
        public List<Supplier> Suppliers { get; } = [];

        public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            Suppliers.Add(supplier);
            return Task.CompletedTask;
        }

        public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ApplySearch(search).Count());
        }

        public Task<Supplier?> GetByCodeAsync(SupplierCode code, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Suppliers.FirstOrDefault(supplier => supplier.Code.Equals(code)));
        }

        public Task<Supplier?> GetByIdAsync(Guid supplierId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Suppliers.FirstOrDefault(supplier => supplier.Id == supplierId));
        }

        public Task<IReadOnlyCollection<Supplier>> SearchAsync(
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var suppliers = ApplySearch(search)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Supplier>>(suppliers);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private IEnumerable<Supplier> ApplySearch(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Suppliers;
            }

            return Suppliers.Where(supplier =>
                supplier.Code.Value.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                supplier.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
    }
}
