namespace ERP.Api.Contracts.MasterData;

public sealed record UpdateProductRequest(
    string Name,
    Guid CategoryId,
    Guid UnitOfMeasureId);
