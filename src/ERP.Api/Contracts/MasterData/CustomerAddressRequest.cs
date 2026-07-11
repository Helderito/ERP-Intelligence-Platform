namespace ERP.Api.Contracts.MasterData;

public sealed record CustomerAddressRequest(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string Country);
