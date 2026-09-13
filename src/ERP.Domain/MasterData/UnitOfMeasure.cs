using ERP.Domain.MasterData.Events;
using ERP.Domain.Tenancy;
using ERP.SharedKernel;

namespace ERP.Domain.MasterData;

public sealed class UnitOfMeasure : Entity<Guid>, ICompanyOwned
{
    private UnitOfMeasure()
        : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public UnitOfMeasure(Guid id, Guid companyId, string code, string name)
        : this(id, companyId, code, name, DateTime.UnixEpoch)
    {
    }

    public UnitOfMeasure(Guid id, Guid companyId, string code, string name, DateTime createdAtUtc)
        : base(id)
    {
        CompanyId = EnsureRequiredCompanyId(companyId);
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public string Code { get; private set; }

    public Guid CompanyId { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? DeactivatedAtUtc { get; private set; }

    public static UnitOfMeasure Create(Guid companyId, string code, string name, DateTime createdAtUtc)
    {
        var unit = new UnitOfMeasure(Guid.NewGuid(), companyId, code, name, createdAtUtc);
        unit.RaiseDomainEvent(new UnitOfMeasureCreated(unit.Id, unit.Code, createdAtUtc));
        return unit;
    }

    private static Guid EnsureRequiredCompanyId(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException("Company identifier is required.", nameof(companyId));
        }

        return companyId;
    }

    public void UpdateDetails(string name, DateTime updatedAtUtc)
    {
        Name = NormalizeName(name);
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
        RaiseDomainEvent(new UnitOfMeasureDeactivated(Id, deactivatedAtUtc));
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Unit of measure code is required.", nameof(code));
        }

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unit of measure name is required.", nameof(name));
        }

        return name.Trim();
    }
}
