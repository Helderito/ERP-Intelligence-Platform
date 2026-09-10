namespace ERP.Application.MasterData.Models;

public sealed record WarehouseDto(
    Guid Id,
    string Code,
    string Name,
    Guid WarehouseTypeId,
    string WarehouseTypeName,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc);
