using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
