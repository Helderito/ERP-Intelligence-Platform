using ERP.Domain.Identity;
using ERP.Domain.Identity.Events;

namespace ERP.UnitTests.Identity;

public sealed class RefreshTokenTests
{
    [Fact]
    public void Issue_ShouldRegisterDomainEvent_WhenTokenIsValid()
    {
        var refreshToken = RefreshToken.Issue(
            Guid.NewGuid(),
            "refresh-token",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow);

        Assert.Contains(refreshToken.DomainEvents, domainEvent => domainEvent is RefreshTokenIssued);
    }

    [Fact]
    public void Revoke_ShouldMarkTokenAsRevoked()
    {
        var refreshToken = RefreshToken.Issue(
            Guid.NewGuid(),
            "refresh-token",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow);

        refreshToken.Revoke(DateTime.UtcNow);

        Assert.True(refreshToken.IsRevoked);
        Assert.Contains(refreshToken.DomainEvents, domainEvent => domainEvent is RefreshTokenRevoked);
    }
}
