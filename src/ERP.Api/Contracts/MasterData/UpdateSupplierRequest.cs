namespace ERP.Api.Contracts.MasterData;

public sealed record UpdateSupplierRequest(
    string Name,
    IReadOnlyCollection<SupplierContactRequest> Contacts,
    IReadOnlyCollection<SupplierAddressRequest> Addresses);
