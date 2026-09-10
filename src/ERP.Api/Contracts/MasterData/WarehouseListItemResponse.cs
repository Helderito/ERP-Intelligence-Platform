using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record WarehouseListItemResponse(Guid Id, string Code, string Name, bool IsActive)
{
    public static WarehouseListItemResponse FromDto(WarehouseListItemDto warehouse)
    {
        return new WarehouseListItemResponse(warehouse.Id, warehouse.Code, warehouse.Name, warehouse.IsActive);
    }
}
