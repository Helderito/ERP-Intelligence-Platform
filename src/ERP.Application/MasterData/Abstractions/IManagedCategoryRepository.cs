using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IManagedCategoryRepository
{
    Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Category>> ListAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
