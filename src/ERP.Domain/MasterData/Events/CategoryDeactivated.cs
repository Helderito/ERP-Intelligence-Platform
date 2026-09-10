using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record CategoryDeactivated(Guid CategoryId, DateTime OccurredAtUtc) : IDomainEvent;
