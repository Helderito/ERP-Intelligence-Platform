using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class ProductCatalogServiceTests
{
    [Fact]
    public async Task CreateProductAsync_ShouldCreateProduct_WhenReferencesExistAndCodeIsUnique()
    {
        var categoryId = Guid.NewGuid();
        var unitOfMeasureId = Guid.NewGuid();
        var productRepository = new FakeProductRepository();
        var service = new ProductCatalogService(
            productRepository,
            new FakeCategoryRepository(categoryId),
            new FakeUnitOfMeasureRepository(unitOfMeasureId));

        var product = await service.CreateProductAsync(
            new CreateProductCommand("sku-001", "Sample Product", categoryId, unitOfMeasureId));

        Assert.Equal("SKU-001", product.Code);
        Assert.Single(productRepository.Products);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldThrow_WhenCodeAlreadyExists()
    {
        var categoryId = Guid.NewGuid();
        var unitOfMeasureId = Guid.NewGuid();
        var productRepository = new FakeProductRepository();
        var service = new ProductCatalogService(
            productRepository,
            new FakeCategoryRepository(categoryId),
            new FakeUnitOfMeasureRepository(unitOfMeasureId));

        await service.CreateProductAsync(new CreateProductCommand("sku-001", "Sample Product", categoryId, unitOfMeasureId));

        await Assert.ThrowsAsync<ProductCodeAlreadyExistsException>(
            () => service.CreateProductAsync(new CreateProductCommand("SKU-001", "Duplicate", categoryId, unitOfMeasureId)));
    }

    [Fact]
    public async Task CreateProductAsync_ShouldThrow_WhenCategoryDoesNotExist()
    {
        var unitOfMeasureId = Guid.NewGuid();
        var service = new ProductCatalogService(
            new FakeProductRepository(),
            new FakeCategoryRepository(),
            new FakeUnitOfMeasureRepository(unitOfMeasureId));

        await Assert.ThrowsAsync<MasterDataReferenceNotFoundException>(
            () => service.CreateProductAsync(
                new CreateProductCommand("sku-001", "Sample Product", Guid.NewGuid(), unitOfMeasureId)));
    }

    [Fact]
    public async Task SearchProductsAsync_ShouldReturnPagedResults()
    {
        var categoryId = Guid.NewGuid();
        var unitOfMeasureId = Guid.NewGuid();
        var productRepository = new FakeProductRepository();
        var service = new ProductCatalogService(
            productRepository,
            new FakeCategoryRepository(categoryId),
            new FakeUnitOfMeasureRepository(unitOfMeasureId));

        await service.CreateProductAsync(new CreateProductCommand("sku-001", "Sample Product", categoryId, unitOfMeasureId));

        var result = await service.SearchProductsAsync(new SearchProductsQuery("sample", 1, 20));

        Assert.Equal(1, result.TotalRecords);
        Assert.Single(result.Items);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; } = [];

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            Products.Add(product);
            return Task.CompletedTask;
        }

        public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ApplySearch(search).Count());
        }

        public Task<Product?> GetByCodeAsync(ProductCode code, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Products.FirstOrDefault(product => product.Code.Equals(code)));
        }

        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Products.FirstOrDefault(product => product.Id == productId));
        }

        public Task<IReadOnlyCollection<Product>> SearchAsync(
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var products = ApplySearch(search)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Product>>(products);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private IEnumerable<Product> ApplySearch(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Products;
            }

            return Products.Where(product =>
                product.Code.Value.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                product.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        private readonly HashSet<Guid> _categoryIds;

        public FakeCategoryRepository(params Guid[] categoryIds)
        {
            _categoryIds = categoryIds.ToHashSet();
        }

        public Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_categoryIds.Contains(categoryId));
        }

        public Task<IReadOnlyCollection<Category>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Category>>(
                _categoryIds.Select(id => new Category(id, $"CAT-{id:N}"[..12], "Category")).ToArray());
        }
    }

    private sealed class FakeUnitOfMeasureRepository : IUnitOfMeasureRepository
    {
        private readonly HashSet<Guid> _unitOfMeasureIds;

        public FakeUnitOfMeasureRepository(params Guid[] unitOfMeasureIds)
        {
            _unitOfMeasureIds = unitOfMeasureIds.ToHashSet();
        }

        public Task<bool> ExistsAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_unitOfMeasureIds.Contains(unitOfMeasureId));
        }

        public Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<UnitOfMeasure>>(
                _unitOfMeasureIds.Select(id => new UnitOfMeasure(id, $"UOM-{id:N}"[..12], "Unit")).ToArray());
        }
    }
}
