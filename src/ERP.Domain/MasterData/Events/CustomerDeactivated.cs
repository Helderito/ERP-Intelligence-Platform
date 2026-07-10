using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record CustomerDeactivated(Guid CustomerId, DateTime OccurredAtUtc) : IDomainEvent;
