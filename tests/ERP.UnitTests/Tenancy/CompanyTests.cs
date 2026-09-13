using ERP.Domain.Tenancy;
using ERP.Domain.Tenancy.Events;

namespace ERP.UnitTests.Tenancy;

public sealed class CompanyTests
{
    [Fact]
    public void Register_ShouldNormalizeNameAndRaiseEvent()
    {
        var company = Company.Register("  Acme Angola  ", DateTime.UtcNow);

        Assert.Equal("Acme Angola", company.Name);
        Assert.True(company.IsActive);
        Assert.IsType<CompanyRegistered>(Assert.Single(company.DomainEvents));
    }

    [Fact]
    public void AddEstablishment_ShouldNormalizeCodeAndRejectDuplicate()
    {
        var company = Company.Register("Acme", DateTime.UtcNow);
        var establishment = company.AddEstablishment(" main ", "Head Office", "001", DateTime.UtcNow);

        Assert.Equal("MAIN", establishment.Code);
        Assert.IsType<EstablishmentAdded>(company.DomainEvents.Last());
        Assert.Throws<InvalidOperationException>(() =>
            company.AddEstablishment("MAIN", "Duplicate", "002", DateTime.UtcNow));
    }

    [Fact]
    public void UpdateFiscalProfile_ShouldNormalizeValuesAndRaiseEvent()
    {
        var company = Company.Register("Acme", DateTime.UtcNow);

        company.UpdateFiscalProfile(
            Nif.Create(" ao-123 456 "),
            VatRegime.General,
            FiscalAddress.Create("  Luanda, Angola  "),
            DateTime.UtcNow);

        Assert.Equal("AO123456", company.FiscalProfile?.Nif.Value);
        Assert.Equal("Luanda, Angola", company.FiscalProfile?.FiscalAddress.Value);
        Assert.IsType<FiscalProfileUpdated>(company.DomainEvents.Last());
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("123456789012345678901")]
    public void Nif_ShouldRejectInvalidValues(string value)
    {
        Assert.Throws<ArgumentException>(() => Nif.Create(value));
    }

    [Fact]
    public void UserCompany_ShouldRequireValidIdentifiers()
    {
        Assert.Throws<ArgumentException>(() =>
            UserCompany.Create(Guid.Empty, TenancySeed.DefaultCompanyId, DateTime.UtcNow));
    }
}
