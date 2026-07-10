using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record ProductDeactivated(Guid ProductId, DateTime OccurredAtUtc) : IDomainEvent;
