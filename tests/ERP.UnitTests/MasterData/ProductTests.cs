using ERP.Domain.MasterData;
using ERP.Domain.MasterData.Events;

namespace ERP.UnitTests.MasterData;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldCreateActiveProductAndRegisterEvent_WhenDataIsValid()
    {
        var product = CreateProduct();

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("SKU-001", product.Code.Value);
        Assert.True(product.IsActive);
        Assert.Contains(product.DomainEvents, domainEvent => domainEvent is ProductCreated);
    }

    [Fact]
    public void UpdateDetails_ShouldNotChangeCode()
    {
        var product = CreateProduct();
        var nextCategoryId = Guid.NewGuid();
        var nextUnitOfMeasureId = Guid.NewGuid();

        product.UpdateDetails("Updated Product", nextCategoryId, nextUnitOfMeasureId, DateTime.UtcNow);

        Assert.Equal("SKU-001", product.Code.Value);
        Assert.Equal("Updated Product", product.Name);
        Assert.Equal(nextCategoryId, product.CategoryId);
        Assert.Equal(nextUnitOfMeasureId, product.UnitOfMeasureId);
    }

    [Fact]
    public void Deactivate_ShouldSoftDeactivateProductAndRegisterEvent()
    {
        var product = CreateProduct();

        product.Deactivate(DateTime.UtcNow);

        Assert.False(product.IsActive);
        Assert.NotNull(product.DeactivatedAtUtc);
        Assert.Contains(product.DomainEvents, domainEvent => domainEvent is ProductDeactivated);
    }

    private static Product CreateProduct()
    {
        return Product.Create(
            ProductCode.Create("sku-001"),
            "Sample Product",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);
    }
}
