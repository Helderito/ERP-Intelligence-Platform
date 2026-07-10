using ERP.Domain.MasterData.Events;
using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class Product : Entity<Guid>
{
    private Product()
        : base(Guid.Empty)
    {
        Code = null!;
        Name = string.Empty;
    }

    private Product(
        Guid id,
        ProductCode code,
        string name,
        Guid categoryId,
        Guid unitOfMeasureId,
        DateTime createdAtUtc)
        : base(id)
    {
        Code = code;
        Name = NormalizeName(name);
        CategoryId = EnsureRequiredId(categoryId, nameof(categoryId));
        UnitOfMeasureId = EnsureRequiredId(unitOfMeasureId, nameof(unitOfMeasureId));
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public ProductCode Code { get; private set; }

    public string Name { get; private set; }

    public Guid CategoryId { get; private set; }

    public Guid UnitOfMeasureId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? DeactivatedAtUtc { get; private set; }

    public static Product Create(
        ProductCode code,
        string name,
        Guid categoryId,
        Guid unitOfMeasureId,
        DateTime createdAtUtc)
    {
        var product = new Product(Guid.NewGuid(), code, name, categoryId, unitOfMeasureId, createdAtUtc);
        product.RaiseDomainEvent(new ProductCreated(product.Id, product.Code.Value, createdAtUtc));

        return product;
    }

    public void UpdateDetails(string name, Guid categoryId, Guid unitOfMeasureId, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        CategoryId = EnsureRequiredId(categoryId, nameof(categoryId));
        UnitOfMeasureId = EnsureRequiredId(unitOfMeasureId, nameof(unitOfMeasureId));
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
        RaiseDomainEvent(new ProductDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));
        }

        return name.Trim();
    }

    private static Guid EnsureRequiredId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O identificador é obrigatório.", parameterName);
        }

        return id;
    }
}
