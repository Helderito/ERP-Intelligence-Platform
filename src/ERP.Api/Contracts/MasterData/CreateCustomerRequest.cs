namespace ERP.Api.Contracts.MasterData;

public sealed record CreateCustomerRequest(
    string Code,
    string Name,
    IReadOnlyCollection<CustomerContactRequest> Contacts,
    IReadOnlyCollection<CustomerAddressRequest> Addresses);
