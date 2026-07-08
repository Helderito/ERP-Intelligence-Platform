using ERP.Domain.Identity.Events;
using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class User : Entity<Guid>
{
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
}
