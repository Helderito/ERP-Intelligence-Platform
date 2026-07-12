using ERP.Application.Common.Models;
using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Models;
using ERP.Application.MasterData.Queries;
using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Services;

public sealed class ProductCatalogService
{
    public const int MaximumPageSize = 100;

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

    public ProductCatalogService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfMeasureRepository unitOfMeasureRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfMeasureRepository = unitOfMeasureRepository;
    }

    public async Task<ProductDto> CreateProductAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureReferencesExistAsync(command.CategoryId, command.UnitOfMeasureId, cancellationToken);

        var code = ProductCode.Create(command.Code);
        var existingProduct = await _productRepository.GetByCodeAsync(code, cancellationToken);

        if (existingProduct is not null)
        {
            throw new ProductCodeAlreadyExistsException(code.Value);
        }

        var product = Product.Create(
            code,
            command.Name,
            command.CategoryId,
            command.UnitOfMeasureId,
            DateTime.UtcNow);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return ToProductDto(product);
    }

    public async Task<ProductDto> UpdateProductAsync(
        UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureReferencesExistAsync(command.CategoryId, command.UnitOfMeasureId, cancellationToken);

        var product = await GetProductOrThrowAsync(command.ProductId, cancellationToken);
        product.UpdateDetails(command.Name, command.CategoryId, command.UnitOfMeasureId, DateTime.UtcNow);

        await _productRepository.SaveChangesAsync(cancellationToken);

        return ToProductDto(product);
    }

    public async Task DeactivateProductAsync(
        DeactivateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrThrowAsync(command.ProductId, cancellationToken);
        product.Deactivate(DateTime.UtcNow);

        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        GetProductByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);

        return product is null ? null : ToProductDto(product);
    }

    public async Task<PagedResultDto<ProductListItemDto>> SearchProductsAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, MaximumPageSize);
        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

        var totalRecords = await _productRepository.CountAsync(search, cancellationToken);
        var products = await _productRepository.SearchAsync(search, page, pageSize, cancellationToken);

        return new PagedResultDto<ProductListItemDto>(
            page,
            pageSize,
            totalRecords,
            products.Select(ToProductListItemDto).ToArray());
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.ListAsync(cancellationToken);

        return categories
            .Select(category => new CategoryDto(category.Id, category.Code, category.Name))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<UnitOfMeasureDto>> GetUnitsOfMeasureAsync(
        CancellationToken cancellationToken = default)
    {
        var units = await _unitOfMeasureRepository.ListAsync(cancellationToken);

        return units
            .Select(unit => new UnitOfMeasureDto(unit.Id, unit.Code, unit.Name))
            .ToArray();
    }

    private async Task<Product> GetProductOrThrowAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException();
        }

        return product;
    }

    private async Task EnsureReferencesExistAsync(
        Guid categoryId,
        Guid unitOfMeasureId,
        CancellationToken cancellationToken)
    {
        if (!await _categoryRepository.ExistsAsync(categoryId, cancellationToken))
        {
            throw new MasterDataReferenceNotFoundException("Category was not found.");
        }

        if (!await _unitOfMeasureRepository.ExistsAsync(unitOfMeasureId, cancellationToken))
        {
            throw new MasterDataReferenceNotFoundException("Unit of measure was not found.");
        }
    }

    private static ProductDto ToProductDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Code.Value,
            product.Name,
            product.CategoryId,
            product.UnitOfMeasureId,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc,
            product.DeactivatedAtUtc);
    }

    private static ProductListItemDto ToProductListItemDto(Product product)
    {
        return new ProductListItemDto(
            product.Id,
            product.Code.Value,
            product.Name,
            product.IsActive);
    }
}
