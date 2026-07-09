using ERP.Domain.Identity;

namespace ERP.Application.Identity.Abstractions;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> GetByIdsAsync(
        IReadOnlyCollection<Guid> roleIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> ListAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task AddAsync(Role role, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
