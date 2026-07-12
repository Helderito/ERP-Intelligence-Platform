namespace ERP.Application.MasterData.Models;

public sealed record SupplierListItemDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);
