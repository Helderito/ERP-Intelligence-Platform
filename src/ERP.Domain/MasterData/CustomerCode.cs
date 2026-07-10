using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class CustomerCode : ValueObject
{
    private CustomerCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static CustomerCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Customer code is required.", nameof(value));
        }

        return new CustomerCode(value.Trim().ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
