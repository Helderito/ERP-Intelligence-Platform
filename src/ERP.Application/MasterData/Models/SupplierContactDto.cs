namespace ERP.Application.MasterData.Models;

public sealed record SupplierContactDto(Guid Id, string Name, string? Email, string? Phone);
