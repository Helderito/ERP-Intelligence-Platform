using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Country : Entity<Guid>
{
    private Country() : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Country(Guid id, string code, string name) : base(id)
    {
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
    }

    public string Code { get; private set; }
    public string Name { get; private set; }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Country code is required.", nameof(code));
        }

        var normalized = code.Trim().ToUpperInvariant();
        if (normalized.Length != 2 || normalized.Any(character => character is < 'A' or > 'Z'))
        {
            throw new ArgumentException("Country code must be a two-letter ISO 3166-1 code.", nameof(code));
        }

        return normalized;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Country name is required.", nameof(name));
        }

        return name.Trim();
    }
}
