using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface IWarehouseTypeRepository
{
    Task<WarehouseType?> GetByIdAsync(Guid warehouseTypeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<WarehouseType>> ListAsync(CancellationToken cancellationToken = default);
}
