using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByCodeAsync(WarehouseCode code, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? search, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Warehouse>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
