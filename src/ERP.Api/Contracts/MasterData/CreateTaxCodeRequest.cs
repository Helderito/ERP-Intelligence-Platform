namespace ERP.Api.Contracts.MasterData;

public sealed record CreateTaxCodeRequest(string Code, string Name, decimal Rate);
