using ERP.Application.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationService>();
        services.AddScoped<AuthorizationService>();

        return services;
    }
}
