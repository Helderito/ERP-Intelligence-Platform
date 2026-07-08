using ERP.Domain.Identity;
using ERP.Domain.Identity.Events;

namespace ERP.UnitTests.Identity;

public sealed class UserTests
{
    [Fact]
    public void Register_ShouldCreateActiveUser_WhenDataIsValid()
    {
        var user = User.Register(
            EmailAddress.Create("user@example.com"),
            PasswordHash.Create("hashed-password"),
            DateTime.UtcNow);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.IsActive);
        Assert.Equal("user@example.com", user.Email.Value);
        Assert.Contains(user.DomainEvents, domainEvent => domainEvent is UserRegistered);
    }

    [Fact]
    public void PasswordHash_ShouldThrow_WhenValueIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => PasswordHash.Create(""));
    }
}
