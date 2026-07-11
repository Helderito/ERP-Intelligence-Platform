namespace ERP.Application.MasterData.Commands;

public sealed record CustomerContactCommand(string Name, string? Email, string? Phone);
