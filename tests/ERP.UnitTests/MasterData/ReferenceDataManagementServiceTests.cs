using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class ReferenceDataManagementServiceTests
{
    [Fact]
    public async Task CreateCategoryAsync_ShouldCreateNormalizedCategory()
    {
        var categoryRepository = new FakeCategoryRepository();
        var service = CreateService(categoryRepository: categoryRepository);

        var result = await service.CreateCategoryAsync(new CreateCategoryCommand("food", "Food"));

        Assert.Equal("FOOD", result.Code);
        Assert.Single(categoryRepository.Items);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldThrowTypedException_WhenCodeExists()
    {
        var repository = new FakeCategoryRepository();
        var service = CreateService(categoryRepository: repository);
        await service.CreateCategoryAsync(new CreateCategoryCommand("FOOD", "Food"));

        await Assert.ThrowsAsync<ReferenceCodeAlreadyExistsException>(() =>
            service.CreateCategoryAsync(new CreateCategoryCommand(" food ", "Duplicate")));
    }

    [Fact]
    public async Task UpdateUnitOfMeasureAsync_ShouldKeepCodeImmutable()
    {
        var repository = new FakeUnitRepository();
        var service = CreateService(unitRepository: repository);
        var created = await service.CreateUnitOfMeasureAsync(new CreateUnitOfMeasureCommand("BOX", "Box"));

        var updated = await service.UpdateUnitOfMeasureAsync(
            new UpdateUnitOfMeasureCommand(created.Id, "Shipping Box"));

        Assert.Equal("BOX", updated.Code);
        Assert.Equal("Shipping Box", updated.Name);
    }

    [Fact]
    public async Task DeactivateCategoryAsync_ShouldThrowTypedException_WhenMissing()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ReferenceItemNotFoundException>(() =>
            service.DeactivateCategoryAsync(new DeactivateCategoryCommand(Guid.NewGuid())));
    }

    [Fact]
    public async Task CreateTaxCodeAsync_ShouldCreateTaxCodeWithRate()
    {
        var repository = new FakeTaxCodeRepository();
        var service = CreateService(taxCodeRepository: repository);

        var result = await service.CreateTaxCodeAsync(new CreateTaxCodeCommand("VAT14", "VAT 14%", 14));

        Assert.Equal(14, result.Rate);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task ListTaxCodesAsync_ShouldApplySearchAndIncludeInactive()
    {
        var repository = new FakeTaxCodeRepository();
        var service = CreateService(taxCodeRepository: repository);
        var created = await service.CreateTaxCodeAsync(new CreateTaxCodeCommand("VAT14", "VAT 14%", 14));
        await service.DeactivateTaxCodeAsync(new DeactivateTaxCodeCommand(created.Id));

        var active = await service.ListTaxCodesAsync(new ListReferenceItemsQuery("VAT", false));
        var all = await service.ListTaxCodesAsync(new ListReferenceItemsQuery("VAT", true));

        Assert.Empty(active);
        Assert.Single(all);
    }

    private static ReferenceDataManagementService CreateService(
        FakeCategoryRepository? categoryRepository = null,
        FakeUnitRepository? unitRepository = null,
        FakeTaxCodeRepository? taxCodeRepository = null)
        => new(
            categoryRepository ?? new FakeCategoryRepository(),
            unitRepository ?? new FakeUnitRepository(),
            taxCodeRepository ?? new FakeTaxCodeRepository(),
            new TestCurrentCompanyProvider());

    private sealed class FakeCategoryRepository : IManagedCategoryRepository
    {
        public List<Category> Items { get; } = [];
        public Task AddAsync(Category item, CancellationToken cancellationToken = default) { Items.Add(item); return Task.CompletedTask; }
        public Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(item => item.Code == code));
        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(item => item.Id == id));
        public Task<IReadOnlyCollection<Category>> ListAsync(string? search, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<Category>>(Items.Where(item => (includeInactive || item.IsActive) && Matches(item.Code, item.Name, search)).ToArray());
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeUnitRepository : IManagedUnitOfMeasureRepository
    {
        public List<UnitOfMeasure> Items { get; } = [];
        public Task AddAsync(UnitOfMeasure item, CancellationToken cancellationToken = default) { Items.Add(item); return Task.CompletedTask; }
        public Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(item => item.Code == code));
        public Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(item => item.Id == id));
        public Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(string? search, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<UnitOfMeasure>>(Items.Where(item => (includeInactive || item.IsActive) && Matches(item.Code, item.Name, search)).ToArray());
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeTaxCodeRepository : ITaxCodeRepository
    {
        public List<TaxCode> Items { get; } = [];
        public Task AddAsync(TaxCode item, CancellationToken cancellationToken = default) { Items.Add(item); return Task.CompletedTask; }
        public Task<TaxCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(item => item.Code == code));
        public Task<TaxCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(item => item.Id == id));
        public Task<IReadOnlyCollection<TaxCode>> ListAsync(string? search, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<TaxCode>>(Items.Where(item => (includeInactive || item.IsActive) && Matches(item.Code, item.Name, search)).ToArray());
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private static bool Matches(string code, string name, string? search)
        => string.IsNullOrWhiteSpace(search) ||
            code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            name.Contains(search, StringComparison.OrdinalIgnoreCase);
}
