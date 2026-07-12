using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid supplierId, CancellationToken cancellationToken = default);

    Task<Supplier?> GetByCodeAsync(SupplierCode code, CancellationToken cancellationToken = default);

    Task<int> CountAsync(string? search, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Supplier>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
