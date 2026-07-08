using ERP.SharedKernel;

namespace ERP.Domain.Identity.Events;

public sealed record RefreshTokenRevoked(Guid RefreshTokenId, Guid UserId, DateTime OccurredAtUtc) : IDomainEvent;
