using ERP.Domain.Identity;

namespace ERP.UnitTests.Identity;

public sealed class PermissionTests
{
    [Fact]
    public void Constructor_ShouldNormalizePermissionCode_WhenDataIsValid()
    {
        var permission = new Permission(Guid.NewGuid(), " Roles.Manage ", "Manage roles");

        Assert.Equal("roles.manage", permission.Code);
        Assert.Equal("Manage roles", permission.Description);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCodeIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Permission(Guid.NewGuid(), "", "Description"));
    }
}
