using System.Security.Claims;
using ERP.Domain.Tenancy;
using Microsoft.AspNetCore.Http;

namespace ERP.Infrastructure.Tenancy;

public sealed class CurrentCompanyProvider : ICurrentCompanyProvider
{
    public const string CompanyIdClaimType = "companyId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentCompanyProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CompanyId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(CompanyIdClaimType);
            return Guid.TryParse(value, out var companyId) ? companyId : null;
        }
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public Guid GetRequiredCompanyId()
    {
        return CompanyId ?? throw new InvalidOperationException("Current company could not be resolved.");
    }
}
