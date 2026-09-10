using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IManagedUnitOfMeasureRepository
{
    Task<UnitOfMeasure?> GetByIdAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default);
    Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task AddAsync(UnitOfMeasure unitOfMeasure, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
