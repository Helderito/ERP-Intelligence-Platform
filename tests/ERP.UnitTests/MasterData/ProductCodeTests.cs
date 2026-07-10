using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class ProductCodeTests
{
    [Fact]
    public void Create_ShouldNormalizeCode_WhenValueIsValid()
    {
        var code = ProductCode.Create(" sku-001 ");

        Assert.Equal("SKU-001", code.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenValueIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => ProductCode.Create(""));
    }
}
