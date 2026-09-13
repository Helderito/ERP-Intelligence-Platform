using ERP.SharedKernel;

namespace ERP.Domain.Tenancy;

public sealed class Establishment : Entity<Guid>
{
    private Establishment() : base(Guid.Empty)
    {
        Code = string.Empty;
        Name = string.Empty;
        EstablishmentNumber = string.Empty;
    }

    internal Establishment(Guid id, Guid companyId, string code, string name, string establishmentNumber)
        : base(id)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException("Company identifier is required.", nameof(companyId));
        }

        CompanyId = companyId;
        Code = NormalizeRequired(code, nameof(code), 50).ToUpperInvariant();
        Name = NormalizeRequired(name, nameof(name), 200);
        EstablishmentNumber = NormalizeRequired(establishmentNumber, nameof(establishmentNumber), 50);
    }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string EstablishmentNumber { get; private set; }

    private static string NormalizeRequired(string value, string parameterName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException($"{parameterName} cannot exceed {maximumLength} characters.", parameterName);
        }

        return normalized;
    }
}
