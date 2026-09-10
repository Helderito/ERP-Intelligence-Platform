using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record UnitOfMeasureDeactivated(Guid UnitOfMeasureId, DateTime OccurredAtUtc) : IDomainEvent;
