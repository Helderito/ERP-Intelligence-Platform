namespace ERP.Application.MasterData.Models;

public sealed record SupplierDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc,
    IReadOnlyCollection<SupplierContactDto> Contacts,
    IReadOnlyCollection<SupplierAddressDto> Addresses);
