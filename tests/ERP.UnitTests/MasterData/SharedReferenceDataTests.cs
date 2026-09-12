using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class SharedReferenceDataTests
{
    [Fact]
    public void Country_ShouldNormalizeCodeAndName_WhenDataIsValid()
    {
        var country = new Country(Guid.NewGuid(), " ao ", " Angola ");

        Assert.Equal("AO", country.Code);
        Assert.Equal("Angola", country.Name);
    }

    [Theory]
    [InlineData("", "Angola")]
    [InlineData("A", "Angola")]
    [InlineData("AGO", "Angola")]
    [InlineData("A1", "Angola")]
    [InlineData("AO", "")]
    public void Country_ShouldRejectInvalidCodeOrName(string code, string name)
    {
        Assert.Throws<ArgumentException>(() => new Country(Guid.NewGuid(), code, name));
    }

    [Fact]
    public void Currency_ShouldNormalizeCodeAndName_WhenDataIsValid()
    {
        var currency = new Currency(Guid.NewGuid(), " aoa ", " Angolan Kwanza ");

        Assert.Equal("AOA", currency.Code);
        Assert.Equal("Angolan Kwanza", currency.Name);
    }

    [Theory]
    [InlineData("", "Euro")]
    [InlineData("EU", "Euro")]
    [InlineData("EURO", "Euro")]
    [InlineData("EU1", "Euro")]
    [InlineData("EUR", "")]
    public void Currency_ShouldRejectInvalidCodeOrName(string code, string name)
    {
        Assert.Throws<ArgumentException>(() => new Currency(Guid.NewGuid(), code, name));
    }

    [Fact]
    public void PaymentTerm_ShouldNormalizeValues_WhenDataIsValid()
    {
        var paymentTerm = new PaymentTerm(Guid.NewGuid(), " net30 ", " Net 30 days ", 30);

        Assert.Equal("NET30", paymentTerm.Code);
        Assert.Equal("Net 30 days", paymentTerm.Name);
        Assert.Equal(30, paymentTerm.NetDays);
    }

    [Theory]
    [InlineData("", "Immediate")]
    [InlineData("NET0", "")]
    public void PaymentTerm_ShouldRejectMissingCodeOrName(string code, string name)
    {
        Assert.Throws<ArgumentException>(() => new PaymentTerm(Guid.NewGuid(), code, name, 0));
    }

    [Fact]
    public void PaymentTerm_ShouldRejectNegativeNetDays()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PaymentTerm(Guid.NewGuid(), "NET30", "Net 30 days", -1));
    }
}
