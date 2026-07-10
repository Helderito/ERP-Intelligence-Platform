namespace ERP.Application.MasterData.Models;

public sealed record ProductDto(
    Guid Id,
    string Code,
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc);
