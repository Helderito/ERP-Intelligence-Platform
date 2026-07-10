namespace ERP.Api.Contracts.MasterData;

public sealed record CustomerContactRequest(string Name, string? Email, string? Phone);
