using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ERP.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Http;

namespace ERP.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=ERPIntelligence";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options, new CurrentCompanyProvider(new HttpContextAccessor()));
    }
}
