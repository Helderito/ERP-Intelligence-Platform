using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record CustomerCreated(Guid CustomerId, string Code, DateTime OccurredAtUtc) : IDomainEvent;
