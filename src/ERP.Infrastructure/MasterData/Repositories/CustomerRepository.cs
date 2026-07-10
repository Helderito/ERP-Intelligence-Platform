using ERP.Application.MasterData.Abstractions;
using ERP.Domain.MasterData;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.MasterData.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers
            .Include(customer => customer.Contacts)
            .Include(customer => customer.Addresses)
            .FirstOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);
    }

    public Task<Customer?> GetByCodeAsync(CustomerCode code, CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers
            .Include(customer => customer.Contacts)
            .Include(customer => customer.Addresses)
            .FirstOrDefaultAsync(customer => customer.Code.Value == code.Value, cancellationToken);
    }

    public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        return ApplySearch(_dbContext.Customers.AsNoTracking(), search)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Customer>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await ApplySearch(_dbContext.Customers.AsNoTracking(), search)
            .Include(customer => customer.Contacts)
            .Include(customer => customer.Addresses)
            .OrderBy(customer => customer.Code.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Customer> ApplySearch(IQueryable<Customer> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var pattern = $"%{search.Trim()}%";

        return query.Where(customer =>
            EF.Functions.ILike(customer.Code.Value, pattern) ||
            EF.Functions.ILike(customer.Name, pattern));
    }
}
