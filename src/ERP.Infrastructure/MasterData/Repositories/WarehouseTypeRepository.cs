using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class WarehouseTypeRepository : IWarehouseTypeRepository
{
    private readonly AppDbContext _dbContext;

    public WarehouseTypeRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public Task<WarehouseType?> GetByIdAsync(Guid warehouseTypeId, CancellationToken cancellationToken = default)
    {
        return _dbContext.WarehouseTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(type => type.Id == warehouseTypeId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<WarehouseType>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.WarehouseTypes
            .AsNoTracking()
            .OrderBy(type => type.Name)
            .ToArrayAsync(cancellationToken);
    }
}
