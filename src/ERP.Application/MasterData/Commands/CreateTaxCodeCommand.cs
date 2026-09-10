namespace ERP.Application.MasterData.Commands;

public sealed record CreateTaxCodeCommand(string Code, string Name, decimal Rate);
