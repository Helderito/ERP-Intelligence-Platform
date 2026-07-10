namespace ERP.Application.MasterData.Commands;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId);
