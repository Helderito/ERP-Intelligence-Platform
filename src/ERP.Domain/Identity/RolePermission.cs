using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class RolePermission : Entity<Guid>
{
    private RolePermission()
        : base(Guid.Empty)
    {
    }

    private RolePermission(Guid id, Guid roleId, Guid permissionId, DateTime assignedAtUtc)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        AssignedAtUtc = assignedAtUtc;
    }

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public DateTime AssignedAtUtc { get; private set; }

    public static RolePermission Create(Guid roleId, Guid permissionId, DateTime assignedAtUtc)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(roleId));
        }

        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException("Permission id is required.", nameof(permissionId));
        }

        return new RolePermission(Guid.NewGuid(), roleId, permissionId, assignedAtUtc);
    }
}
