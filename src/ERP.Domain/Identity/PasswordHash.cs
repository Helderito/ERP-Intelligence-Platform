namespace ERP.Domain.Identity;

public sealed class PasswordHash : IEquatable<PasswordHash>
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

    public bool Equals(PasswordHash? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PasswordHash);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return Value;
    }
}
