using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record SupplierResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc,
    IReadOnlyCollection<SupplierContactResponse> Contacts,
    IReadOnlyCollection<SupplierAddressResponse> Addresses)
{
    public static SupplierResponse FromDto(SupplierDto supplier)
    {
        return new SupplierResponse(
            supplier.Id,
            supplier.Code,
            supplier.Name,
            supplier.IsActive,
            supplier.CreatedAtUtc,
            supplier.UpdatedAtUtc,
            supplier.DeactivatedAtUtc,
            supplier.Contacts.Select(SupplierContactResponse.FromDto).ToArray(),
            supplier.Addresses.Select(SupplierAddressResponse.FromDto).ToArray());
    }
}
