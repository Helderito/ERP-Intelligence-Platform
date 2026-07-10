using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record ProductCreated(Guid ProductId, string ProductCode, DateTime OccurredAtUtc) : IDomainEvent;
