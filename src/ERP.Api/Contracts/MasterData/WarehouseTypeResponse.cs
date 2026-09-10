using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record WarehouseTypeResponse(Guid Id, string Code, string Name)
{
    public static WarehouseTypeResponse FromDto(WarehouseTypeDto warehouseType)
    {
        return new WarehouseTypeResponse(warehouseType.Id, warehouseType.Code, warehouseType.Name);
    }
}
