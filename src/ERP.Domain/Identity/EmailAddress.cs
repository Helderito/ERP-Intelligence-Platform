using System.Net.Mail;
using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class EmailAddress : ValueObject
{
    private EmailAddress(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email address is required.", nameof(value));
        }

        var normalizedValue = value.Trim().ToLowerInvariant();

        try
        {
            var mailAddress = new MailAddress(normalizedValue);

            if (!string.Equals(mailAddress.Address, normalizedValue, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Email address format is invalid.", nameof(value));
            }
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Email address format is invalid.", nameof(value), ex);
        }

        return new EmailAddress(normalizedValue);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
