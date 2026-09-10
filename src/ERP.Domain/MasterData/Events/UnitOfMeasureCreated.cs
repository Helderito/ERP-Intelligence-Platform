using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record UnitOfMeasureCreated(Guid UnitOfMeasureId, string Code, DateTime OccurredAtUtc) : IDomainEvent;
