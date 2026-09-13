using ERP.SharedKernel;

namespace ERP.Domain.Tenancy.Events;

public sealed record CompanyRegistered(Guid CompanyId, string Name, DateTime OccurredAtUtc) : IDomainEvent;
