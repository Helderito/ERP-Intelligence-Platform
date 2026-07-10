using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly AppDbContext _dbContext;

    public UnitOfMeasureRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UnitsOfMeasure.AnyAsync(unit => unit.Id == unitOfMeasureId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.UnitsOfMeasure
            .AsNoTracking()
            .OrderBy(unit => unit.Name)
            .ToArrayAsync(cancellationToken);
    }
}
