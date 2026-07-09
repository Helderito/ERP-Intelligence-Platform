using ERP.Application.Identity.Abstractions;
using ERP.Application.Identity.Commands;
using ERP.Application.Identity.Models;
using ERP.Application.Identity.Services;
using ERP.Domain.Identity;

namespace ERP.UnitTests.Identity;

public sealed class AuthenticationServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndIssueTokens_WhenDataIsValid()
    {
        var userRepository = new FakeUserRepository();
        var refreshTokenRepository = new FakeRefreshTokenRepository();
        var service = new AuthenticationService(
            userRepository,
            refreshTokenRepository,
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator(),
            new AuthorizationService(new FakeRoleRepository(), new FakePermissionRepository(), userRepository));

        var result = await service.RegisterAsync(new RegisterUserCommand("user@example.com", "password"));

        Assert.Equal("user@example.com", result.Email);
        Assert.Single(userRepository.Users);
        Assert.Single(refreshTokenRepository.RefreshTokens);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
    }

    [Fact]
    public async Task RegisterAsync_ShouldAssignAdministratorRole_OnlyToTheFirstUser()
    {
        var userRepository = new FakeUserRepository();
        var service = new AuthenticationService(
            userRepository,
            new FakeRefreshTokenRepository(),
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator(),
            new AuthorizationService(new FakeRoleRepository(), new FakePermissionRepository(), userRepository));

        await service.RegisterAsync(new RegisterUserCommand("first@example.com", "password"));
        await service.RegisterAsync(new RegisterUserCommand("second@example.com", "password"));

        var firstUser = userRepository.Users[0];
        var secondUser = userRepository.Users[1];

        Assert.Contains(IdentitySeed.AdministratorRoleId, firstUser.RoleIds);
        Assert.Empty(secondUser.RoleIds);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsInvalid()
    {
        var userRepository = new FakeUserRepository();
        var refreshTokenRepository = new FakeRefreshTokenRepository();
        var service = new AuthenticationService(
            userRepository,
            refreshTokenRepository,
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator(),
            new AuthorizationService(new FakeRoleRepository(), new FakePermissionRepository(), userRepository));

        await service.RegisterAsync(new RegisterUserCommand("user@example.com", "password"));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(new LoginCommand("user@example.com", "wrong-password")));
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.Any(user => user.Email.Equals(email)));
        }

        public Task<bool> HasAnyAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.Count > 0);
        }

        public Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Email.Equals(email)));
        }

        public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Id == userId));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
    {
        public List<RefreshToken> RefreshTokens { get; } = [];

        public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            RefreshTokens.Add(refreshToken);
            return Task.CompletedTask;
        }

        public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(RefreshTokens.FirstOrDefault(refreshToken => refreshToken.Token == token));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRoleRepository : IRoleRepository
    {
        public Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Role?>(null);
        }

        public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Role?>(null);
        }

        public Task<IReadOnlyCollection<Role>> GetByIdsAsync(
            IReadOnlyCollection<Guid> roleIds,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Role>>([]);
        }

        public Task<IReadOnlyCollection<Role>> ListAsync(
            bool includeInactive,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Role>>([]);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakePermissionRepository : IPermissionRepository
    {
        public Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Permission?>(null);
        }

        public Task<Permission?> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Permission?>(null);
        }

        public Task<IReadOnlyCollection<Permission>> GetByIdsAsync(
            IReadOnlyCollection<Guid> permissionIds,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Permission>>([]);
        }

        public Task<IReadOnlyCollection<Permission>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Permission>>([]);
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        public AccessTokenResult Generate(User user, IReadOnlyCollection<string> roleNames)
        {
            return new AccessTokenResult($"token:{user.Id}", DateTime.UtcNow.AddMinutes(15));
        }

        public Guid? Validate(string accessToken)
        {
            return null;
        }
    }
}
