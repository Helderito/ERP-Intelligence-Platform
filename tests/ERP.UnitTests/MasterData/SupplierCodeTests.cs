using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class SupplierCodeTests
{
    [Fact]
    public void Create_ShouldNormalizeCode_WhenValueIsValid()
    {
        var code = SupplierCode.Create(" sup-001 ");

        Assert.Equal("SUP-001", code.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenValueIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => SupplierCode.Create(""));
    }
}
