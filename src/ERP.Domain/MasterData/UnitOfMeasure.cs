using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class UnitOfMeasure : Entity<Guid>
{
    private UnitOfMeasure()
        : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public UnitOfMeasure(Guid id, string code, string name)
        : base(id)
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
            throw new ArgumentException("Unit of measure code is required.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unit of measure name is required.", nameof(name));
        }

        return name.Trim();
    }
}
