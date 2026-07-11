using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record CustomerAddressResponse(
    Guid Id,
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string Country)
{
    public static CustomerAddressResponse FromDto(CustomerAddressDto address)
    {
        return new CustomerAddressResponse(
            address.Id,
            address.Line1,
            address.Line2,
            address.City,
            address.PostalCode,
            address.Country);
    }
}
