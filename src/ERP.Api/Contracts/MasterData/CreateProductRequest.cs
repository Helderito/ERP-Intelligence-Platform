namespace ERP.Api.Contracts.MasterData;

public sealed record CreateProductRequest(
    string Code,
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId);
