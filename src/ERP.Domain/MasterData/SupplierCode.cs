using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class SupplierCode : ValueObject
{
    private SupplierCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static SupplierCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Supplier code is required.", nameof(value));
        }

        return new SupplierCode(value.Trim().ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
