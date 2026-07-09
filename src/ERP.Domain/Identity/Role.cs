using ERP.Domain.Identity.Events;
using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class Role : Entity<Guid>
{
    private readonly List<RolePermission> _rolePermissions = [];

    private Role()
        : base(Guid.Empty)
    {
        Name = string.Empty;
    }

    private Role(Guid id, string name, DateTime createdAtUtc)
        : base(id)
    {
        Name = NormalizeName(name);
        CreatedAtUtc = createdAtUtc;
        IsActive = true;
    }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? DeactivatedAtUtc { get; private set; }

    public IReadOnlyCollection<Guid> PermissionIds => _rolePermissions
        .Select(rolePermission => rolePermission.PermissionId)
        .ToArray();

    public static Role Create(string name, DateTime createdAtUtc)
    {
        var role = new Role(Guid.NewGuid(), name, createdAtUtc);
        role.RaiseDomainEvent(new RoleCreated(role.Id, role.Name, createdAtUtc));

        return role;
    }

    public void UpdateName(string name, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime deactivatedAtUtc)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        DeactivatedAtUtc = deactivatedAtUtc;
        UpdatedAtUtc = deactivatedAtUtc;
    }

    public void AssignPermission(Guid permissionId, DateTime assignedAtUtc)
    {
        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException("Permission id is required.", nameof(permissionId));
        }

        if (_rolePermissions.Any(rolePermission => rolePermission.PermissionId == permissionId))
        {
            return;
        }

        _rolePermissions.Add(RolePermission.Create(Id, permissionId, assignedAtUtc));
        RaiseDomainEvent(new PermissionAssigned(Id, permissionId, assignedAtUtc));
    }

    public void RevokePermission(Guid permissionId, DateTime revokedAtUtc)
    {
        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException("Permission id is required.", nameof(permissionId));
        }

        var rolePermission = _rolePermissions.FirstOrDefault(item => item.PermissionId == permissionId);

        if (rolePermission is not null)
        {
            _rolePermissions.Remove(rolePermission);
            RaiseDomainEvent(new PermissionRevoked(Id, permissionId, revokedAtUtc));
        }
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required.", nameof(name));
        }

        return name.Trim();
    }
}
