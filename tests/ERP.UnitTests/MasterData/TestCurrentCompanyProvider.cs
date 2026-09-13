using ERP.Domain.Tenancy;

namespace ERP.UnitTests.MasterData;

internal sealed class TestCurrentCompanyProvider : ICurrentCompanyProvider
{
    public TestCurrentCompanyProvider(Guid? companyId = null)
    {
        CompanyId = companyId ?? TenancySeed.DefaultCompanyId;
    }

    public Guid? CompanyId { get; }

    public Guid? UserId => null;

    public Guid GetRequiredCompanyId()
        => CompanyId ?? throw new InvalidOperationException("Current company is required for this test.");
}
