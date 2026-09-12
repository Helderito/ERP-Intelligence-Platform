namespace ERP.Application.MasterData.Models;

public sealed record PaymentTermDto(Guid Id, string Code, string Name, int NetDays);
