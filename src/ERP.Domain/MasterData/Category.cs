using ERP.Domain.MasterData.Events;
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
        : this(id, code, name, DateTime.UnixEpoch)
    {
    }

    public Category(Guid id, string code, string name, DateTime createdAtUtc)
        : base(id)
    {
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? DeactivatedAtUtc { get; private set; }

    public static Category Create(string code, string name, DateTime createdAtUtc)
    {
        var category = new Category(Guid.NewGuid(), code, name, createdAtUtc);
        category.RaiseDomainEvent(new CategoryCreated(category.Id, category.Code, createdAtUtc));
        return category;
    }

    public void UpdateDetails(string name, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime deactivatedAtUtc)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        DeactivatedAtUtc = deactivatedAtUtc;
        UpdatedAtUtc = deactivatedAtUtc;
        RaiseDomainEvent(new CategoryDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Category code is required.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(name));
        }

        return name.Trim();
    }
}
