namespace ERP.Application.MasterData.Commands;

public sealed record UpdateWarehouseCommand(Guid WarehouseId, string Name, Guid WarehouseTypeId);
