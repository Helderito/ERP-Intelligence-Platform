namespace ERP.Api.Contracts.MasterData;

public sealed record UpdateWarehouseRequest(string Name, Guid WarehouseTypeId);
