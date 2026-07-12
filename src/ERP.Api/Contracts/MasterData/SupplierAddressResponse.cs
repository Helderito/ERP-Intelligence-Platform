using ERP.Application.MasterData.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record SupplierAddressResponse(
    Guid Id,
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string Country)
{
    public static SupplierAddressResponse FromDto(SupplierAddressDto address)
    {
        return new SupplierAddressResponse(
            address.Id,
            address.Line1,
            address.Line2,
            address.City,
            address.PostalCode,
            address.Country);
    }
}
