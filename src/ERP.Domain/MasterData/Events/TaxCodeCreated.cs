using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record TaxCodeCreated(Guid TaxCodeId, string Code, DateTime OccurredAtUtc) : IDomainEvent;
