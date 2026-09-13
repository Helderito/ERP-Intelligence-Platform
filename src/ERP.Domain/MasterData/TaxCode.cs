using ERP.Domain.MasterData.Events;
using ERP.Domain.Tenancy;
using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class TaxCode : Entity<Guid>, ICompanyOwned
{
    private TaxCode() : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    private TaxCode(Guid id, Guid companyId, string code, string name, decimal rate, DateTime createdAtUtc) : base(id)
    {
        CompanyId = EnsureRequiredCompanyId(companyId);
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        Rate = ValidateRate(rate);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public string Code { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public decimal Rate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? DeactivatedAtUtc { get; private set; }

    public static TaxCode Create(Guid companyId, string code, string name, decimal rate, DateTime createdAtUtc)
    {
        var taxCode = new TaxCode(Guid.NewGuid(), companyId, code, name, rate, createdAtUtc);
        taxCode.RaiseDomainEvent(new TaxCodeCreated(taxCode.Id, taxCode.Code, createdAtUtc));
        return taxCode;
    }

    private static Guid EnsureRequiredCompanyId(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException("Company identifier is required.", nameof(companyId));
        }

        return companyId;
    }

    public void UpdateDetails(string name, decimal rate, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        Rate = ValidateRate(rate);
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
        RaiseDomainEvent(new TaxCodeDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Tax code is required.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tax code name is required.", nameof(name));
        }

        return name.Trim();
    }

    private static decimal ValidateRate(decimal rate)
    {
        if (rate is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), rate, "Tax rate must be between 0 and 100.");
        }

        return rate;
    }
}
