using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface ITaxCodeRepository
{
    Task<TaxCode?> GetByIdAsync(Guid taxCodeId, CancellationToken cancellationToken = default);
    Task<TaxCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaxCode>> ListAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task AddAsync(TaxCode taxCode, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
