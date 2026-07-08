using ERP.SharedKernel;

namespace ERP.Domain.Identity;

public sealed class PasswordHash : ValueObject
{
    private PasswordHash(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password hash is required.", nameof(value));
        }

        return new PasswordHash(value);
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
