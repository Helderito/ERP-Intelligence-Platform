namespace ERP.Api.Contracts.MasterData;

public sealed record CreateSupplierRequest(
    string Code,
    string Name,
    IReadOnlyCollection<SupplierContactRequest> Contacts,
    IReadOnlyCollection<SupplierAddressRequest> Addresses);
