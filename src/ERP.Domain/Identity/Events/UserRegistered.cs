namespace ERP.Domain.Identity.Events;

public sealed record UserRegistered(Guid UserId, string Email, DateTime OccurredAtUtc);
