namespace ERP.Application.MasterData.Models;

public sealed record ReferenceDataItemDto(Guid Id, string Code, string Name, bool IsActive);
