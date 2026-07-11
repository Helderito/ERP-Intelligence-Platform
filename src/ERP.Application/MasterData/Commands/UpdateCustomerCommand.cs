namespace ERP.Application.MasterData.Commands;

public sealed record UpdateCustomerCommand(
    Guid CustomerId,
    string Name,
    IReadOnlyCollection<CustomerContactCommand> Contacts,
    IReadOnlyCollection<CustomerAddressCommand> Addresses);
