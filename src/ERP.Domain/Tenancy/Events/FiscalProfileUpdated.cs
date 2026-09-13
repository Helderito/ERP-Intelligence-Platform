using ERP.SharedKernel;

namespace ERP.Domain.Tenancy.Events;

public sealed record FiscalProfileUpdated(Guid CompanyId, DateTime OccurredAtUtc) : IDomainEvent;
