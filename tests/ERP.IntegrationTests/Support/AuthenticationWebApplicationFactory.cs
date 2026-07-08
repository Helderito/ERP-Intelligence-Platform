using ERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace ERP.IntegrationTests.Support;

public sealed class AuthenticationWebApplicationFactory :
    WebApplicationFactory<Program>,
    IAsyncLifetime
{
    private const string JwtSigningKey = "integration-tests-signing-key-with-at-least-32-characters";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("ERPIntelligenceTests")
        .WithUsername("erp")
        .WithPassword("erp_test_password")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _postgres.GetConnectionString());
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", null);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", null);
        await _postgres.DisposeAsync();
    }
}
