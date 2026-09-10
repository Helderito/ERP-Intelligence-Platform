using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _dbContext;

    public WarehouseRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public Task<Warehouse?> GetByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Warehouses
            .Include(warehouse => warehouse.WarehouseType)
            .FirstOrDefaultAsync(warehouse => warehouse.Id == warehouseId, cancellationToken);
    }

    public Task<Warehouse?> GetByCodeAsync(WarehouseCode code, CancellationToken cancellationToken = default)
    {
        return _dbContext.Warehouses
            .Include(warehouse => warehouse.WarehouseType)
            .FirstOrDefaultAsync(warehouse => warehouse.Code.Value == code.Value, cancellationToken);
    }

    public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        return ApplySearch(_dbContext.Warehouses.AsNoTracking(), search).CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Warehouse>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await ApplySearch(_dbContext.Warehouses.AsNoTracking(), search)
            .OrderBy(warehouse => warehouse.Code.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        await _dbContext.Warehouses.AddAsync(warehouse, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Warehouse> ApplySearch(IQueryable<Warehouse> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var pattern = $"%{search.Trim()}%";
        return query.Where(warehouse =>
            EF.Functions.ILike(warehouse.Code.Value, pattern) ||
            EF.Functions.ILike(warehouse.Name, pattern));
    }
}
