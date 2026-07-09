using ERP.SharedKernel;

namespace ERP.Domain.Identity.Events;

public sealed record RoleCreated(Guid RoleId, string Name, DateTime OccurredAtUtc) : IDomainEvent;
