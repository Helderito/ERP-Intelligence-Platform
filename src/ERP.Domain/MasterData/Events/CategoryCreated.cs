using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record CategoryCreated(Guid CategoryId, string CategoryCode, DateTime OccurredAtUtc) : IDomainEvent;
