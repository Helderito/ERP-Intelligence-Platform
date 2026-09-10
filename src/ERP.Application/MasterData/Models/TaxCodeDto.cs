namespace ERP.Application.MasterData.Models;

public sealed record TaxCodeDto(
    Guid Id,
    string Code,
    string Name,
    decimal Rate,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc);
