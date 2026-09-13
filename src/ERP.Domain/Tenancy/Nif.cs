using ERP.SharedKernel;

namespace ERP.Domain.Tenancy;

public sealed class Nif : ValueObject
{
    private Nif(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Nif Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("NIF is required.", nameof(value));
        }

        var normalized = new string(value
            .Where(char.IsLetterOrDigit)
            .Select(char.ToUpperInvariant)
            .ToArray());

        if (normalized.Length is < 6 or > 20)
        {
            throw new ArgumentException("NIF must contain between 6 and 20 alphanumeric characters.", nameof(value));
        }

        return new Nif(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
