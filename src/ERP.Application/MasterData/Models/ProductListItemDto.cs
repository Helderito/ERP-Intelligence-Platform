namespace ERP.Application.MasterData.Models;

public sealed record ProductListItemDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);
