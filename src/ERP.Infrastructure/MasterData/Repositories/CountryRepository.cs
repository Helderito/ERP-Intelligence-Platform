using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class CountryRepository : ICountryRepository
{
    private readonly AppDbContext _dbContext;

    public CountryRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<Country>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Countries
            .AsNoTracking()
            .OrderBy(country => country.Code)
            .ToArrayAsync(cancellationToken);
    }
}
