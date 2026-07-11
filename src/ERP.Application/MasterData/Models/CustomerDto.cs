namespace ERP.Application.MasterData.Models;

public sealed record CustomerDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc,
    IReadOnlyCollection<CustomerContactDto> Contacts,
    IReadOnlyCollection<CustomerAddressDto> Addresses);
