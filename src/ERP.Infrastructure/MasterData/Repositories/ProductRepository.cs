using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }

    public Task<Product?> GetByCodeAsync(ProductCode code, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.FirstOrDefaultAsync(
            product => product.Code.Value == code.Value,
            cancellationToken);
    }

    public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        return ApplySearch(_dbContext.Products.AsNoTracking(), search)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await ApplySearch(_dbContext.Products.AsNoTracking(), search)
            .OrderBy(product => product.Code.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Product> ApplySearch(IQueryable<Product> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var pattern = $"%{search.Trim()}%";

        return query.Where(product =>
            EF.Functions.ILike(product.Code.Value, pattern) ||
            EF.Functions.ILike(product.Name, pattern));
    }
}
