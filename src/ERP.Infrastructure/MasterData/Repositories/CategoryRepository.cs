using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AnyAsync(category => category.Id == categoryId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToArrayAsync(cancellationToken);
    }
}
