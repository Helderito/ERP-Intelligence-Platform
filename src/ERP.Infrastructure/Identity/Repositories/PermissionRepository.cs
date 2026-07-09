using ERP.Application.Identity.Abstractions;
using ERP.Domain.Identity;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Identity.Repositories;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _dbContext;

    public PermissionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Permissions.FirstOrDefaultAsync(
            permission => permission.Id == permissionId,
            cancellationToken);
    }

    public Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();

        return _dbContext.Permissions.FirstOrDefaultAsync(
            permission => permission.Code == normalizedCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> GetByIdsAsync(
        IReadOnlyCollection<Guid> permissionIds,
        CancellationToken cancellationToken = default)
    {
        if (permissionIds.Count == 0)
        {
            return [];
        }

        return await _dbContext.Permissions
            .Where(permission => permissionIds.Contains(permission.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .OrderBy(permission => permission.Code)
            .ToArrayAsync(cancellationToken);
    }
}
