namespace ERP.Api.Contracts.MasterData;

public sealed record UpdateCustomerRequest(
    string Name,
    IReadOnlyCollection<CustomerContactRequest> Contacts,
    IReadOnlyCollection<CustomerAddressRequest> Addresses);
