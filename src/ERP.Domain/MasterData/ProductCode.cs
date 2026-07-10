using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class ProductCode : ValueObject
{
    private ProductCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ProductCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("O código do produto é obrigatório.", nameof(value));
        }

        return new ProductCode(value.Trim().ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
