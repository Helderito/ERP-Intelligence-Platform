using ERP.Domain.MasterData;
using ERP.Domain.MasterData.Events;

namespace ERP.UnitTests.MasterData;

public sealed class ManagedReferenceDataTests
{
    [Theory]
    [InlineData("", "Food")]
    [InlineData("   ", "Food")]
    [InlineData("FOOD", "")]
    [InlineData("FOOD", "   ")]
    public void Category_ShouldRejectMissingCodeOrName(string code, string name)
    {
        Assert.Throws<ArgumentException>(() => Category.Create(code, name, DateTime.UtcNow));
    }

    [Fact]
    public void Category_ShouldNormalizeImmutableCodeAndSoftDeactivate()
    {
        var category = Category.Create(" food ", "Food", DateTime.UtcNow);

        category.UpdateDetails("Fresh Food", DateTime.UtcNow);
        category.Deactivate(DateTime.UtcNow);
        var eventCount = category.DomainEvents.Count;
        category.Deactivate(DateTime.UtcNow.AddMinutes(1));

        Assert.Equal("FOOD", category.Code);
        Assert.Equal("Fresh Food", category.Name);
        Assert.False(category.IsActive);
        Assert.Contains(category.DomainEvents, item => item is CategoryCreated);
        Assert.Contains(category.DomainEvents, item => item is CategoryDeactivated);
        Assert.Equal(eventCount, category.DomainEvents.Count);
    }

    [Fact]
    public void UnitOfMeasure_ShouldNormalizeImmutableCodeAndSoftDeactivate()
    {
        var unit = UnitOfMeasure.Create(" box ", "Box", DateTime.UtcNow);

        unit.UpdateDetails("Shipping Box", DateTime.UtcNow);
        unit.Deactivate(DateTime.UtcNow);
        var eventCount = unit.DomainEvents.Count;
        unit.Deactivate(DateTime.UtcNow.AddMinutes(1));

        Assert.Equal("BOX", unit.Code);
        Assert.Equal("Shipping Box", unit.Name);
        Assert.False(unit.IsActive);
        Assert.Contains(unit.DomainEvents, item => item is UnitOfMeasureCreated);
        Assert.Contains(unit.DomainEvents, item => item is UnitOfMeasureDeactivated);
        Assert.Equal(eventCount, unit.DomainEvents.Count);
    }

    [Theory]
    [InlineData("", "Box")]
    [InlineData("   ", "Box")]
    [InlineData("BOX", "")]
    [InlineData("BOX", "   ")]
    public void UnitOfMeasure_ShouldRejectMissingCodeOrName(string code, string name)
    {
        Assert.Throws<ArgumentException>(() => UnitOfMeasure.Create(code, name, DateTime.UtcNow));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public void TaxCode_ShouldRejectRateOutsidePercentageRange(decimal rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TaxCode.Create("VAT", "VAT", rate, DateTime.UtcNow));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(14)]
    [InlineData(100)]
    public void TaxCode_ShouldAcceptRateInsidePercentageRange(decimal rate)
    {
        var taxCode = TaxCode.Create(" vat ", "VAT", rate, DateTime.UtcNow);

        Assert.Equal("VAT", taxCode.Code);
        Assert.Equal(rate, taxCode.Rate);
        Assert.True(taxCode.IsActive);
        Assert.Contains(taxCode.DomainEvents, item => item is TaxCodeCreated);
    }

    [Fact]
    public void TaxCode_UpdateAndDeactivate_ShouldKeepCodeAndRaiseDeactivationEvent()
    {
        var taxCode = TaxCode.Create("VAT", "VAT", 14, DateTime.UtcNow);

        taxCode.UpdateDetails("Value Added Tax", 15, DateTime.UtcNow);
        taxCode.Deactivate(DateTime.UtcNow);
        var eventCount = taxCode.DomainEvents.Count;
        taxCode.Deactivate(DateTime.UtcNow.AddMinutes(1));

        Assert.Equal("VAT", taxCode.Code);
        Assert.Equal("Value Added Tax", taxCode.Name);
        Assert.Equal(15, taxCode.Rate);
        Assert.False(taxCode.IsActive);
        Assert.Contains(taxCode.DomainEvents, item => item is TaxCodeDeactivated);
        Assert.Equal(eventCount, taxCode.DomainEvents.Count);
    }

    [Theory]
    [InlineData("", "VAT")]
    [InlineData("   ", "VAT")]
    [InlineData("VAT", "")]
    [InlineData("VAT", "   ")]
    public void TaxCode_ShouldRejectMissingCodeOrName(string code, string name)
    {
        Assert.Throws<ArgumentException>(() => TaxCode.Create(code, name, 14, DateTime.UtcNow));
    }
}
