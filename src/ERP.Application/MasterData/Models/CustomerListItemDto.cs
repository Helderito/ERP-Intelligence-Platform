namespace ERP.Application.MasterData.Models;

public sealed record CustomerListItemDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);
