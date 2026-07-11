using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class CustomerCodeTests
{
    [Fact]
    public void Create_ShouldNormalizeCode_WhenValueIsValid()
    {
        var code = CustomerCode.Create(" cus-001 ");

        Assert.Equal("CUS-001", code.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenValueIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => CustomerCode.Create(""));
    }
}
