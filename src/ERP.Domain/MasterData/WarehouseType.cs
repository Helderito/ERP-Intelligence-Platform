using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class WarehouseType : Entity<Guid>
{
    private WarehouseType() : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public WarehouseType(Guid id, string code, string name) : base(id)
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
            throw new ArgumentException("Warehouse type code is required.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Warehouse type name is required.", nameof(name));
        }

        return name.Trim();
    }
}
