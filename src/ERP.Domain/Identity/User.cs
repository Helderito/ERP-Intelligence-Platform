using ERP.Domain.Identity.Events;
using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class User : Entity<Guid>
{
    private readonly List<UserRole> _userRoles = [];

    private User()
        : base(Guid.Empty)
    {
        Email = null!;
        PasswordHash = null!;
    }

    private User(Guid id, EmailAddress email, PasswordHash passwordHash, DateTime createdAtUtc)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
        IsActive = true;
    }

    public EmailAddress Email { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? LastAuthenticatedAtUtc { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Guid> RoleIds => _userRoles
        .Select(userRole => userRole.RoleId)
        .ToArray();

    public static User Register(EmailAddress email, PasswordHash passwordHash, DateTime createdAtUtc)
    {
        var user = new User(Guid.NewGuid(), email, passwordHash, createdAtUtc);
        user.RaiseDomainEvent(new UserRegistered(user.Id, user.Email.Value, createdAtUtc));

        return user;
    }

    public void MarkAuthenticated(DateTime authenticatedAtUtc)
    {
        LastAuthenticatedAtUtc = authenticatedAtUtc;
        RaiseDomainEvent(new UserAuthenticated(Id, authenticatedAtUtc));
    }

    public void AssignRole(Guid roleId, DateTime assignedAtUtc)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("Role id is required.", nameof(roleId));
        }

        if (_userRoles.Any(userRole => userRole.RoleId == roleId))
        {
            return;
        }

        _userRoles.Add(UserRole.Create(Id, roleId, assignedAtUtc));
        RaiseDomainEvent(new RoleAssignedToUser(Id, roleId, assignedAtUtc));
    }
}
