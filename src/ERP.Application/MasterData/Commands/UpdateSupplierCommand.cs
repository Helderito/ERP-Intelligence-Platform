namespace ERP.Application.MasterData.Commands;

public sealed record UpdateSupplierCommand(
    Guid SupplierId,
    string Name,
    IReadOnlyCollection<SupplierContactCommand> Contacts,
    IReadOnlyCollection<SupplierAddressCommand> Addresses);
