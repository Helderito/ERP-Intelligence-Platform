using ERP.Domain.Identity.Events;
using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class RefreshToken : Entity<Guid>
{
    private RefreshToken()
        : base(Guid.Empty)
    {
        Token = string.Empty;
    }

    private RefreshToken(Guid id, Guid userId, string token, DateTime expiresAtUtc, DateTime createdAtUtc)
        : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; private set; }

    public string Token { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsExpired(DateTime utcNow)
    {
        return ExpiresAtUtc <= utcNow;
    }

    public bool IsActive(DateTime utcNow)
    {
        return !IsRevoked && !IsExpired(utcNow);
    }

    public static RefreshToken Issue(Guid userId, string token, DateTime expiresAtUtc, DateTime createdAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Refresh token is required.", nameof(token));
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentException("Refresh token expiration must be after creation.", nameof(expiresAtUtc));
        }

        var refreshToken = new RefreshToken(Guid.NewGuid(), userId, token, expiresAtUtc, createdAtUtc);
        refreshToken.RaiseDomainEvent(new RefreshTokenIssued(refreshToken.Id, refreshToken.UserId, createdAtUtc));

        return refreshToken;
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        if (IsRevoked)
        {
            return;
        }

        RevokedAtUtc = revokedAtUtc;
        RaiseDomainEvent(new RefreshTokenRevoked(Id, UserId, revokedAtUtc));
    }
}
