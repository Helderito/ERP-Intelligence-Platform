using ERP.SharedKernel;

namespace ERP.Domain.Identity.Events;

public sealed record RoleAssignedToUser(Guid UserId, Guid RoleId, DateTime OccurredAtUtc) : IDomainEvent;
