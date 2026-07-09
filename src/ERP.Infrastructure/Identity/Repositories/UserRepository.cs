using ERP.Application.Identity.Abstractions;
using ERP.Domain.Identity;
using ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Identity.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include("_userRoles")
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include("_userRoles")
            .FirstOrDefaultAsync(user => user.Email.Value == email.Value, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(user => user.Email.Value == email.Value, cancellationToken);
    }

    public Task<bool> HasAnyAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
