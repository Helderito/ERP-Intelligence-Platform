using ERP.Domain.MasterData;

namespace ERP.Application.MasterData.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<Customer?> GetByCodeAsync(CustomerCode code, CancellationToken cancellationToken = default);

    Task<int> CountAsync(string? search, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Customer>> SearchAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
