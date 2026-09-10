using ERP.SharedKernel;

namespace ERP.Domain.MasterData.Events;

public sealed record TaxCodeDeactivated(Guid TaxCodeId, DateTime OccurredAtUtc) : IDomainEvent;
