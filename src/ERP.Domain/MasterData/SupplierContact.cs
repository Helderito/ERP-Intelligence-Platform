using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class SupplierContact : Entity<Guid>
{
    private SupplierContact()
        : base(Guid.Empty)
    {
        Name = string.Empty;
    }

    internal SupplierContact(
        Guid id,
        Guid supplierId,
        string name,
        string? email,
        string? phone)
        : base(id)
    {
        SupplierId = EnsureRequiredId(supplierId, nameof(supplierId));
        Name = NormalizeRequiredText(name, nameof(name), "Contact name is required.");
        Email = NormalizeEmail(email);
        Phone = NormalizeOptionalText(phone);
    }

    public Guid SupplierId { get; private set; }

    public string Name { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    private static Guid EnsureRequiredId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Identifier is required.", parameterName);
        }

        return id;
    }

    private static string NormalizeRequiredText(string value, string parameterName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeEmail(string? value)
    {
        var email = NormalizeOptionalText(value);

        if (email is null)
        {
            return null;
        }

        var atIndex = email.IndexOf('@', StringComparison.Ordinal);
        var dotIndex = email.LastIndexOf(".", StringComparison.Ordinal);

        if (atIndex <= 0 || dotIndex <= atIndex + 1 || dotIndex == email.Length - 1)
        {
            throw new ArgumentException("Contact email format is invalid.", nameof(value));
        }

        return email;
    }
}
