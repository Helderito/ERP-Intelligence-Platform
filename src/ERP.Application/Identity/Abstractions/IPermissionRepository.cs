using ERP.Domain.Identity;

namespace ERP.Application.Identity.Abstractions;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default);

    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permission>> GetByIdsAsync(
        IReadOnlyCollection<Guid> permissionIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permission>> ListAsync(CancellationToken cancellationToken = default);
}
