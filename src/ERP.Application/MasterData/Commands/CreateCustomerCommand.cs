namespace ERP.Application.MasterData.Commands;

public sealed record CreateCustomerCommand(
    string Code,
    string Name,
    IReadOnlyCollection<CustomerContactCommand> Contacts,
    IReadOnlyCollection<CustomerAddressCommand> Addresses);
