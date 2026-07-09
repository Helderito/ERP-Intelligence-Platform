using ERP.Domain.Identity;
using ERP.Domain.Identity.Events;

namespace ERP.UnitTests.Identity;

public sealed class RoleTests
{
    [Fact]
    public void Create_ShouldCreateActiveRoleAndRegisterEvent_WhenNameIsValid()
    {
        var role = Role.Create("Administrators", DateTime.UtcNow);

        Assert.NotEqual(Guid.Empty, role.Id);
        Assert.Equal("Administrators", role.Name);
        Assert.True(role.IsActive);
        Assert.Contains(role.DomainEvents, domainEvent => domainEvent is RoleCreated);
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => Role.Create("", DateTime.UtcNow));
    }

    [Fact]
    public void AssignPermission_ShouldAddPermissionAndRegisterEvent_WhenPermissionIsNew()
    {
        var role = Role.Create("Administrators", DateTime.UtcNow);
        var permissionId = Guid.NewGuid();

        role.AssignPermission(permissionId, DateTime.UtcNow);

        Assert.Contains(permissionId, role.PermissionIds);
        Assert.Contains(role.DomainEvents, domainEvent => domainEvent is PermissionAssigned);
    }

    [Fact]
    public void Deactivate_ShouldSoftDeactivateRole()
    {
        var role = Role.Create("Administrators", DateTime.UtcNow);

        role.Deactivate(DateTime.UtcNow);

        Assert.False(role.IsActive);
        Assert.NotNull(role.DeactivatedAtUtc);
    }
}
