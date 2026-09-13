using ERP.Application.Identity.Services;
using ERP.Application.MasterData.Services;
using ERP.Application.Tenancy.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationService>();
        services.AddScoped<AuthorizationService>();
        services.AddScoped<ProductCatalogService>();
        services.AddScoped<CustomerManagementService>();
        services.AddScoped<SupplierManagementService>();
        services.AddScoped<WarehouseManagementService>();
        services.AddScoped<ReferenceDataManagementService>();
        services.AddScoped<SharedReferenceDataService>();
        services.AddScoped<CompanyManagementService>();

        return services;
    }
}
