namespace ERP.Application.MasterData.Commands;

public sealed record CreateProductCommand(
    string Code,
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId);
