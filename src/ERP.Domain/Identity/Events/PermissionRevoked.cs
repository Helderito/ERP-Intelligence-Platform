using ERP.SharedKernel;

namespace ERP.Domain.Identity.Events;

public sealed record PermissionRevoked(Guid RoleId, Guid PermissionId, DateTime OccurredAtUtc) : IDomainEvent;
