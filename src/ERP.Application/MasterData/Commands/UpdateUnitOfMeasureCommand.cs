namespace ERP.Application.MasterData.Commands;

public sealed record UpdateUnitOfMeasureCommand(Guid UnitOfMeasureId, string Name);
