namespace ERP.Application.MasterData.Models;

public sealed record CustomerContactDto(Guid Id, string Name, string? Email, string? Phone);
