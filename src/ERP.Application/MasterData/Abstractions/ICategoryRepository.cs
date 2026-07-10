using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Category>> ListAsync(CancellationToken cancellationToken = default);
}
