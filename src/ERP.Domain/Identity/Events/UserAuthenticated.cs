namespace ERP.Domain.Identity.Events;

public sealed record UserAuthenticated(Guid UserId, DateTime OccurredAtUtc);
