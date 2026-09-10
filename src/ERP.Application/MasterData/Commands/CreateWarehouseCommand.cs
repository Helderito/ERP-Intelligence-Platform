namespace ERP.Application.MasterData.Commands;

public sealed record CreateWarehouseCommand(string Code, string Name, Guid WarehouseTypeId);
