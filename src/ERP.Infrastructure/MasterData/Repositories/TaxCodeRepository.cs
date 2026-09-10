using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class TaxCodeRepository : ITaxCodeRepository
{
    private readonly AppDbContext _dbContext;

    public TaxCodeRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public Task<TaxCode?> GetByIdAsync(Guid taxCodeId, CancellationToken cancellationToken = default)
        => _dbContext.TaxCodes.FirstOrDefaultAsync(taxCode => taxCode.Id == taxCodeId, cancellationToken);

    public Task<TaxCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => _dbContext.TaxCodes.FirstOrDefaultAsync(taxCode => taxCode.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<TaxCode>> ListAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TaxCodes.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(taxCode => taxCode.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(taxCode =>
                EF.Functions.ILike(taxCode.Code, pattern) || EF.Functions.ILike(taxCode.Name, pattern));
        }

        return await query.OrderBy(taxCode => taxCode.Code).ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(TaxCode taxCode, CancellationToken cancellationToken = default)
        => await _dbContext.TaxCodes.AddAsync(taxCode, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
