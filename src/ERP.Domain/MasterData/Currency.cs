using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Currency : Entity<Guid>
{
    private Currency() : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Currency(Guid id, string code, string name) : base(id)
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
            throw new ArgumentException("Currency code is required.", nameof(code));
        }

        var normalized = code.Trim().ToUpperInvariant();
        if (normalized.Length != 3 || normalized.Any(character => character is < 'A' or > 'Z'))
        {
            throw new ArgumentException("Currency code must be a three-letter ISO 4217 code.", nameof(code));
        }

        return normalized;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Currency name is required.", nameof(name));
        }

        return name.Trim();
    }
}
