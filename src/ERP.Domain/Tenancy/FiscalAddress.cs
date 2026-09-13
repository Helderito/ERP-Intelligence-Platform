using ERP.SharedKernel;

namespace ERP.Domain.Tenancy;

public sealed class FiscalAddress : ValueObject
{
    private FiscalAddress(string value) => Value = value;

    public string Value { get; }

    public static FiscalAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Fiscal address is required.", nameof(value));
        }

        var normalized = value.Trim();
        if (normalized.Length > 500)
        {
            throw new ArgumentException("Fiscal address cannot exceed 500 characters.", nameof(value));
        }

        return new FiscalAddress(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
