using ERP.SharedKernel;

namespace ERP.Domain.Identity.Events;

public sealed record PermissionAssigned(Guid RoleId, Guid PermissionId, DateTime OccurredAtUtc) : IDomainEvent;
