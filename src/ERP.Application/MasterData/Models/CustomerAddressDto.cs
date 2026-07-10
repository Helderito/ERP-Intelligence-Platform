namespace ERP.Application.MasterData.Models;

public sealed record CustomerAddressDto(
    Guid Id,
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string Country);
