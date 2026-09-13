namespace ERP.Domain.Tenancy;

public interface ICurrentCompanyProvider
{
    Guid? UserId { get; }

    Guid? CompanyId { get; }

    Guid GetRequiredCompanyId();
}
