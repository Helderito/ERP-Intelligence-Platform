using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class CategoryRepository : ICategoryRepository, IManagedCategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AnyAsync(
            category => category.Id == categoryId && category.IsActive,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);

    public Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => _dbContext.Categories.FirstOrDefaultAsync(category => category.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<Category>> ListAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(category => category.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(category =>
                EF.Functions.ILike(category.Code, pattern) || EF.Functions.ILike(category.Name, pattern));
        }

        return await query.OrderBy(category => category.Code).ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        => await _dbContext.Categories.AddAsync(category, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
