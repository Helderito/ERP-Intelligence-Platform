using ERP.Domain.Tenancy.Events;
using ERP.SharedKernel;

namespace ERP.Domain.Tenancy;

public sealed class Company : Entity<Guid>
{
    private readonly List<Establishment> _establishments = [];

    private Company() : base(Guid.Empty)
    {
        Name = string.Empty;
    }

    private Company(Guid id, string name, DateTime createdAtUtc) : base(id)
    {
        Name = NormalizeName(name);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? DeactivatedAtUtc { get; private set; }
    public CompanyFiscalProfile? FiscalProfile { get; private set; }
    public IReadOnlyCollection<Establishment> Establishments => _establishments.AsReadOnly();

    public static Company Register(string name, DateTime registeredAtUtc)
    {
        var company = new Company(Guid.NewGuid(), name, registeredAtUtc);
        company.RaiseDomainEvent(new CompanyRegistered(company.Id, company.Name, registeredAtUtc));
        return company;
    }

    public void UpdateDetails(string name, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public Establishment AddEstablishment(
        string code,
        string name,
        string establishmentNumber,
        DateTime addedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Establishment code is required.", nameof(code));
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (_establishments.Any(item => item.Code == normalizedCode))
        {
            throw new InvalidOperationException($"Establishment code '{normalizedCode}' already exists for this company.");
        }

        var establishment = new Establishment(Guid.NewGuid(), Id, code, name, establishmentNumber);
        _establishments.Add(establishment);
        RaiseDomainEvent(new EstablishmentAdded(Id, establishment.Id, establishment.Code, addedAtUtc));
        return establishment;
    }

    public void UpdateFiscalProfile(
        Nif nif,
        VatRegime vatRegime,
        FiscalAddress fiscalAddress,
        DateTime updatedAtUtc)
    {
        FiscalProfile = CompanyFiscalProfile.Create(nif, vatRegime, fiscalAddress);
        UpdatedAtUtc = updatedAtUtc;
        RaiseDomainEvent(new FiscalProfileUpdated(Id, updatedAtUtc));
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
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Company name is required.", nameof(name));
        }

        var normalized = name.Trim();
        if (normalized.Length > 200)
        {
            throw new ArgumentException("Company name cannot exceed 200 characters.", nameof(name));
        }

        return normalized;
    }
}
