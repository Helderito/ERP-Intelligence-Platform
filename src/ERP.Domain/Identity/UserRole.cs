using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class UserRole : Entity<Guid>
{
    private UserRole()
        : base(Guid.Empty)
    {
    }

    private UserRole(Guid id, Guid userId, Guid roleId, DateTime assignedAtUtc)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAtUtc = assignedAtUtc;
    }

    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    public DateTime AssignedAtUtc { get; private set; }

    public static UserRole Create(Guid userId, Guid roleId, DateTime assignedAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(roleId));
        }

        return new UserRole(Guid.NewGuid(), userId, roleId, assignedAtUtc);
    }
}
