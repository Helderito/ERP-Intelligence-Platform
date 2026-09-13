using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class WarehouseManagementServiceTests
{
    [Fact]
    public async Task CreateWarehouseAsync_ShouldCreateWarehouse_WhenTypeExistsAndCodeIsUnique()
    {
        var type = CreateWarehouseType();
        var repository = new FakeWarehouseRepository();
        var service = new WarehouseManagementService(
            repository,
            new FakeWarehouseTypeRepository(type),
            new TestCurrentCompanyProvider());

        var result = await service.CreateWarehouseAsync(
            new CreateWarehouseCommand("main-01", "Main Warehouse", type.Id));

        Assert.Equal("MAIN-01", result.Code);
        Assert.Equal(type.Name, result.WarehouseTypeName);
        Assert.Single(repository.Warehouses);
    }

    [Fact]
    public async Task CreateWarehouseAsync_ShouldThrowTypedException_WhenCodeAlreadyExists()
    {
        var type = CreateWarehouseType();
        var repository = new FakeWarehouseRepository();
        var service = new WarehouseManagementService(
            repository,
            new FakeWarehouseTypeRepository(type),
            new TestCurrentCompanyProvider());
        await service.CreateWarehouseAsync(new CreateWarehouseCommand("main-01", "Main", type.Id));

        await Assert.ThrowsAsync<WarehouseCodeAlreadyExistsException>(() =>
            service.CreateWarehouseAsync(new CreateWarehouseCommand("MAIN-01", "Duplicate", type.Id)));
    }

    [Fact]
    public async Task CreateWarehouseAsync_ShouldThrow_WhenWarehouseTypeDoesNotExist()
    {
        var service = new WarehouseManagementService(
            new FakeWarehouseRepository(),
            new FakeWarehouseTypeRepository(),
            new TestCurrentCompanyProvider());

        await Assert.ThrowsAsync<MasterDataReferenceNotFoundException>(() =>
            service.CreateWarehouseAsync(new CreateWarehouseCommand("MAIN-01", "Main", Guid.NewGuid())));
    }

    [Fact]
    public async Task UpdateWarehouseAsync_ShouldThrowTypedException_WhenWarehouseDoesNotExist()
    {
        var type = CreateWarehouseType();
        var service = new WarehouseManagementService(
            new FakeWarehouseRepository(),
            new FakeWarehouseTypeRepository(type),
            new TestCurrentCompanyProvider());

        await Assert.ThrowsAsync<WarehouseNotFoundException>(() =>
            service.UpdateWarehouseAsync(new UpdateWarehouseCommand(Guid.NewGuid(), "Missing", type.Id)));
    }

    [Fact]
    public async Task SearchWarehousesAsync_ShouldReturnLightPagedResults()
    {
        var type = CreateWarehouseType();
        var repository = new FakeWarehouseRepository();
        var service = new WarehouseManagementService(
            repository,
            new FakeWarehouseTypeRepository(type),
            new TestCurrentCompanyProvider());
        await service.CreateWarehouseAsync(new CreateWarehouseCommand("main-01", "Main Warehouse", type.Id));

        var result = await service.SearchWarehousesAsync(new SearchWarehousesQuery("main", 1, 200));

        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(WarehouseManagementService.MaximumPageSize, result.PageSize);
        Assert.Single(result.Items);
    }

    private static WarehouseType CreateWarehouseType()
    {
        return new WarehouseType(Guid.NewGuid(), "MAIN", "Main Warehouse");
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        public List<Warehouse> Warehouses { get; } = [];

        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
        {
            Warehouses.Add(warehouse);
            return Task.CompletedTask;
        }

        public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
            => Task.FromResult(ApplySearch(search).Count());

        public Task<Warehouse?> GetByCodeAsync(WarehouseCode code, CancellationToken cancellationToken = default)
            => Task.FromResult(Warehouses.FirstOrDefault(warehouse => warehouse.Code.Equals(code)));

        public Task<Warehouse?> GetByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
            => Task.FromResult(Warehouses.FirstOrDefault(warehouse => warehouse.Id == warehouseId));

        public Task<IReadOnlyCollection<Warehouse>> SearchAsync(
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Warehouse>>(
                ApplySearch(search).Skip((page - 1) * pageSize).Take(pageSize).ToArray());
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        private IEnumerable<Warehouse> ApplySearch(string? search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? Warehouses
                : Warehouses.Where(warehouse =>
                    warehouse.Code.Value.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    warehouse.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
    }

    private sealed class FakeWarehouseTypeRepository : IWarehouseTypeRepository
    {
        private readonly WarehouseType[] _warehouseTypes;

        public FakeWarehouseTypeRepository(params WarehouseType[] warehouseTypes)
        {
            _warehouseTypes = warehouseTypes;
        }

        public Task<WarehouseType?> GetByIdAsync(Guid warehouseTypeId, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouseTypes.FirstOrDefault(type => type.Id == warehouseTypeId));

        public Task<IReadOnlyCollection<WarehouseType>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<WarehouseType>>(_warehouseTypes);
    }
}
