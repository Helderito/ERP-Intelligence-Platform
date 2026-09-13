using ERP.SharedKernel;

namespace ERP.Domain.Tenancy.Events;

public sealed record EstablishmentAdded(Guid CompanyId, Guid EstablishmentId, string Code, DateTime OccurredAtUtc)
    : IDomainEvent;
