using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class SupplierAddress : Entity<Guid>
{
    private SupplierAddress()
        : base(Guid.Empty)
    {
        Line1 = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;
        Country = string.Empty;
    }

    internal SupplierAddress(
        Guid id,
        Guid supplierId,
        string line1,
        string? line2,
        string city,
        string postalCode,
        string country)
        : base(id)
    {
        SupplierId = EnsureRequiredId(supplierId, nameof(supplierId));
        Line1 = NormalizeRequiredText(line1, nameof(line1), "Address line 1 is required.");
        Line2 = NormalizeOptionalText(line2);
        City = NormalizeRequiredText(city, nameof(city), "Address city is required.");
        PostalCode = NormalizeRequiredText(postalCode, nameof(postalCode), "Address postal code is required.");
        Country = NormalizeRequiredText(country, nameof(country), "Address country is required.");
    }

    public Guid SupplierId { get; private set; }

    public string Line1 { get; private set; }

    public string? Line2 { get; private set; }

    public string City { get; private set; }

    public string PostalCode { get; private set; }

    public string Country { get; private set; }

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
}
