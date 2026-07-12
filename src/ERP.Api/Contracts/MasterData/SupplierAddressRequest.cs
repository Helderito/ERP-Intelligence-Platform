namespace ERP.Api.Contracts.MasterData;

public sealed record SupplierAddressRequest(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string Country);
