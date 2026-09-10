using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class UnitOfMeasureRepository : IUnitOfMeasureRepository, IManagedUnitOfMeasureRepository
{
    private readonly AppDbContext _dbContext;

    public UnitOfMeasureRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UnitsOfMeasure.AnyAsync(
            unit => unit.Id == unitOfMeasureId && unit.IsActive,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.UnitsOfMeasure
            .AsNoTracking()
            .Where(unit => unit.IsActive)
            .OrderBy(unit => unit.Name)
            .ToArrayAsync(cancellationToken);
    }

    public Task<UnitOfMeasure?> GetByIdAsync(Guid unitOfMeasureId, CancellationToken cancellationToken = default)
        => _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(unit => unit.Id == unitOfMeasureId, cancellationToken);

    public Task<UnitOfMeasure?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(unit => unit.Code == code, cancellationToken);

    public async Task<IReadOnlyCollection<UnitOfMeasure>> ListAsync(
        string? search,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.UnitsOfMeasure.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(unit => unit.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(unit =>
                EF.Functions.ILike(unit.Code, pattern) || EF.Functions.ILike(unit.Name, pattern));
        }

        return await query.OrderBy(unit => unit.Code).ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(UnitOfMeasure unitOfMeasure, CancellationToken cancellationToken = default)
        => await _dbContext.UnitsOfMeasure.AddAsync(unitOfMeasure, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
