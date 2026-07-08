using ERP.Domain.Identity.Events;

namespace ERP.Domain.Identity;

public sealed class User
{
    private readonly List<object> _domainEvents = [];

    private User()
    {
        Email = null!;
        PasswordHash = null!;
    }

    private User(Guid id, EmailAddress email, PasswordHash passwordHash, DateTime createdAtUtc)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public EmailAddress Email { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? LastAuthenticatedAtUtc { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

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

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void RaiseDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
