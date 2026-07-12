using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _dbContext;

    public SupplierRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Supplier?> GetByIdAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Suppliers
            .Include(supplier => supplier.Contacts)
            .Include(supplier => supplier.Addresses)
            .FirstOrDefaultAsync(supplier => supplier.Id == supplierId, cancellationToken);
    }

    public Task<Supplier?> GetByCodeAsync(SupplierCode code, CancellationToken cancellationToken = default)
    {
        return _dbContext.Suppliers
            .Include(supplier => supplier.Contacts)
            .Include(supplier => supplier.Addresses)
            .FirstOrDefaultAsync(supplier => supplier.Code.Value == code.Value, cancellationToken);
    }

    public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        return ApplySearch(_dbContext.Suppliers.AsNoTracking(), search)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Supplier>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await ApplySearch(_dbContext.Suppliers.AsNoTracking(), search)
            .OrderBy(supplier => supplier.Code.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        await _dbContext.Suppliers.AddAsync(supplier, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Supplier> ApplySearch(IQueryable<Supplier> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var pattern = $"%{search.Trim()}%";

        return query.Where(supplier =>
            EF.Functions.ILike(supplier.Code.Value, pattern) ||
            EF.Functions.ILike(supplier.Name, pattern));
    }
}
