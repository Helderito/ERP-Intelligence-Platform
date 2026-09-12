using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class CurrencyRepository : ICurrencyRepository
{
    private readonly AppDbContext _dbContext;

    public CurrencyRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<Currency>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Currencies
            .AsNoTracking()
            .OrderBy(currency => currency.Code)
            .ToArrayAsync(cancellationToken);
    }
}
