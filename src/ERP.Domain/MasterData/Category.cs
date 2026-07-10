using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Category : Entity<Guid>
{
    private Category()
        : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Category(Guid id, string code, string name)
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
            throw new ArgumentException("O código da categoria é obrigatório.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome da categoria é obrigatório.", nameof(name));
        }

        return name.Trim();
    }
}
