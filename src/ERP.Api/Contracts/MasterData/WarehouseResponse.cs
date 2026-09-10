using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record WarehouseResponse(
    Guid Id,
    string Code,
    string Name,
    Guid WarehouseTypeId,
    string WarehouseTypeName,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc)
{
    public static WarehouseResponse FromDto(WarehouseDto warehouse)
    {
        return new WarehouseResponse(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            warehouse.WarehouseTypeId,
            warehouse.WarehouseTypeName,
            warehouse.IsActive,
            warehouse.CreatedAtUtc,
            warehouse.UpdatedAtUtc,
            warehouse.DeactivatedAtUtc);
    }
}
