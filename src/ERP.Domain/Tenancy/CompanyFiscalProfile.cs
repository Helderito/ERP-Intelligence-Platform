using ERP.SharedKernel;

namespace ERP.Domain.Tenancy;

public sealed class CompanyFiscalProfile : ValueObject
{
    private CompanyFiscalProfile()
    {
        Nif = null!;
        FiscalAddress = null!;
    }

    private CompanyFiscalProfile(Nif nif, VatRegime vatRegime, FiscalAddress fiscalAddress)
    {
        Nif = nif;
        VatRegime = vatRegime;
        FiscalAddress = fiscalAddress;
    }

    public Nif Nif { get; private set; }

    public VatRegime VatRegime { get; private set; }

    public FiscalAddress FiscalAddress { get; private set; }

    public static CompanyFiscalProfile Create(Nif nif, VatRegime vatRegime, FiscalAddress fiscalAddress)
    {
        if (!Enum.IsDefined(vatRegime))
        {
            throw new ArgumentOutOfRangeException(nameof(vatRegime), vatRegime, "VAT regime is invalid.");
        }

        return new CompanyFiscalProfile(nif, vatRegime, fiscalAddress);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Nif;
        yield return VatRegime;
        yield return FiscalAddress;
    }
}
