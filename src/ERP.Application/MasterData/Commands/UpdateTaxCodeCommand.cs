namespace ERP.Application.MasterData.Commands;

public sealed record UpdateTaxCodeCommand(Guid TaxCodeId, string Name, decimal Rate);
