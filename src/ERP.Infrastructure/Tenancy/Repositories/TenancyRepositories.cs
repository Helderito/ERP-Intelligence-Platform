using ERP.Application.Tenancy.Abstractions;
using ERP.Domain.Tenancy;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Tenancy.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _dbContext;

    public CompanyRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<Company>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Companies
            .AsNoTracking()
            .OrderBy(company => company.Name)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Companies
            .Include(company => company.Establishments)
            .FirstOrDefaultAsync(company => company.Id == companyId, cancellationToken);
    }

    public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        await _dbContext.Companies.AddAsync(company, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}

public sealed class UserCompanyRepository : IUserCompanyRepository
{
    private readonly AppDbContext _dbContext;

    public UserCompanyRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public Task<Guid?> GetCompanyIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserCompanies
            .AsNoTracking()
            .Where(membership => membership.UserId == userId)
            .Select(membership => (Guid?)membership.CompanyId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(UserCompany membership, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserCompanies.AddAsync(membership, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
