namespace ERP.Application.MasterData.Commands;

public sealed record UpdateCategoryCommand(Guid CategoryId, string Name);
