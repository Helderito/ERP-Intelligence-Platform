using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class WarehouseCodeTests
{
    [Fact]
    public void Create_ShouldNormalizeCode_WhenValueIsValid()
    {
        var code = WarehouseCode.Create(" main-01 ");
        Assert.Equal("MAIN-01", code.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenValueIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => WarehouseCode.Create(" "));
    }
}
