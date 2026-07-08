using ERP.SharedKernel;

namespace ERP.Domain.Identity.Events;

public sealed record RefreshTokenIssued(Guid RefreshTokenId, Guid UserId, DateTime OccurredAtUtc) : IDomainEvent;
