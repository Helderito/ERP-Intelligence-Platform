using ERP.Application.Identity.Abstractions;
using ERP.Domain.Identity;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Identity.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _dbContext;

    public RoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return RolesWithPermissions()
            .FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);
    }

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();

        return RolesWithPermissions()
            .FirstOrDefaultAsync(role => role.Name == normalizedName, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetByIdsAsync(
        IReadOnlyCollection<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        if (roleIds.Count == 0)
        {
            return [];
        }

        return await RolesWithPermissions()
            .Where(role => roleIds.Contains(role.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> ListAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = RolesWithPermissions();

        if (!includeInactive)
        {
            query = query.Where(role => role.IsActive);
        }

        return await query
            .OrderBy(role => role.Name)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(role, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Role> RolesWithPermissions()
    {
        return _dbContext.Roles.Include("_rolePermissions");
    }
}
