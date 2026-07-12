namespace ERP.Application.MasterData.Commands;

public sealed record SupplierAddressCommand(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string Country);
