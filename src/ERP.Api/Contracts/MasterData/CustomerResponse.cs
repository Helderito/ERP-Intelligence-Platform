using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CustomerResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeactivatedAtUtc,
    IReadOnlyCollection<CustomerContactResponse> Contacts,
    IReadOnlyCollection<CustomerAddressResponse> Addresses)
{
    public static CustomerResponse FromDto(CustomerDto customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Code,
            customer.Name,
            customer.IsActive,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc,
            customer.DeactivatedAtUtc,
            customer.Contacts.Select(CustomerContactResponse.FromDto).ToArray(),
            customer.Addresses.Select(CustomerAddressResponse.FromDto).ToArray());
    }
}
