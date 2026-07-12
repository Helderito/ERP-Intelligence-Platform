namespace ERP.Application.MasterData.Commands;

public sealed record CreateSupplierCommand(
    string Code,
    string Name,
    IReadOnlyCollection<SupplierContactCommand> Contacts,
    IReadOnlyCollection<SupplierAddressCommand> Addresses);
