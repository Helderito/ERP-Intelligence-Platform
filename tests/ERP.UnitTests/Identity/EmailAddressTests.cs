using ERP.Domain.Identity;

namespace ERP.UnitTests.Identity;

public sealed class EmailAddressTests
{
    [Fact]
    public void Create_ShouldNormalizeEmail_WhenEmailIsValid()
    {
        var email = EmailAddress.Create(" User@Example.COM ");

        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmailIsInvalid()
    {
        Assert.Throws<ArgumentException>(() => EmailAddress.Create("not-an-email"));
    }
}
