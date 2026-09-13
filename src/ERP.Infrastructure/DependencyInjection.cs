using ERP.Infrastructure.Persistence;
using ERP.Application.Identity.Abstractions;
using ERP.Infrastructure.Identity;
using ERP.Infrastructure.Identity.Authorization;
using ERP.Infrastructure.Identity.Options;
using ERP.Infrastructure.Identity.Repositories;
using ERP.Application.MasterData.Abstractions;
using ERP.Infrastructure.MasterData.Repositories;
using ERP.Application.Tenancy.Abstractions;
using ERP.Domain.Tenancy;
using ERP.Infrastructure.Tenancy;
using ERP.Infrastructure.Tenancy.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentCompanyProvider, CurrentCompanyProvider>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<CategoryRepository>();
        services.AddScoped<ICategoryRepository>(provider => provider.GetRequiredService<CategoryRepository>());
        services.AddScoped<IManagedCategoryRepository>(provider => provider.GetRequiredService<CategoryRepository>());
        services.AddScoped<UnitOfMeasureRepository>();
        services.AddScoped<IUnitOfMeasureRepository>(provider => provider.GetRequiredService<UnitOfMeasureRepository>());
        services.AddScoped<IManagedUnitOfMeasureRepository>(provider => provider.GetRequiredService<UnitOfMeasureRepository>());
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IWarehouseTypeRepository, WarehouseTypeRepository>();
        services.AddScoped<ITaxCodeRepository, TaxCodeRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IPaymentTermRepository, PaymentTermRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IUserCompanyRepository, UserCompanyRepository>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}
