using ERP.Domain.Tenancy;

namespace ERP.Application.Tenancy.Abstractions;

public interface ICompanyRepository
{
    Task<IReadOnlyCollection<Company>> ListAsync(CancellationToken cancellationToken = default);
    Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task AddAsync(Company company, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IUserCompanyRepository
{
    Task<Guid?> GetCompanyIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserCompany membership, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
