namespace ERP.Domain.Tenancy;

public interface ICompanyOwned
{
    Guid CompanyId { get; }
}
