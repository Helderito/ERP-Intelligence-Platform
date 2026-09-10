using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class WarehouseCode : ValueObject
{
    private WarehouseCode(string value) => Value = value;

    public string Value { get; }

    public static WarehouseCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Warehouse code is required.", nameof(value));
        }

        return new WarehouseCode(value.Trim().ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
