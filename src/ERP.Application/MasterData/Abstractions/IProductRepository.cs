using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<Product?> GetByCodeAsync(ProductCode code, CancellationToken cancellationToken = default);

    Task<int> CountAsync(string? search, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Product>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
