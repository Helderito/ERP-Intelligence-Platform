using System.Security.Cryptography;
using ERP.Application.Identity.Abstractions;
using ERP.Application.Identity.Commands;
using ERP.Application.Identity.Models;
using ERP.Application.Identity.Queries;
using ERP.Domain.Identity;

namespace ERP.Application.Identity.Services;

public sealed class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthorizationService _authorizationService;

    public AuthenticationService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        AuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _authorizationService = authorizationService;
    }

    public async Task<AuthenticationResult> RegisterAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        EnsurePasswordIsNotEmpty(command.Password);

        var email = EmailAddress.Create(command.Email);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var passwordHash = PasswordHash.Create(_passwordHasher.Hash(command.Password));
        var utcNow = DateTime.UtcNow;
        var isFirstUser = !await _userRepository.HasAnyAsync(cancellationToken);
        var user = User.Register(email, passwordHash, utcNow);

        // Bootstrap: the very first registered user becomes the Administrator,
        // otherwise no one could ever manage roles (every management endpoint
        // requires a permission that is only reachable through the seeded
        // Administrator role).
        if (isFirstUser)
        {
            user.AssignRole(IdentitySeed.AdministratorRoleId, utcNow);
        }

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return await IssueAuthenticationResultAsync(user, cancellationToken);
    }

    public async Task<AuthenticationResult> LoginAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        EnsurePasswordIsNotEmpty(command.Password);

        var email = EmailAddress.Create(command.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash.Value))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        user.MarkAuthenticated(DateTime.UtcNow);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return await IssueAuthenticationResultAsync(user, cancellationToken);
    }

    public async Task LogoutAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return;
        }

        refreshToken.Revoke(DateTime.UtcNow);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuthenticationResult> RefreshAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive(DateTime.UtcNow))
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        refreshToken.Revoke(DateTime.UtcNow);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return await IssueAuthenticationResultAsync(user, cancellationToken);
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Current user was not found.");
        }

        var roles = await _authorizationService.ListUserRolesAsync(user, cancellationToken);
        var permissions = await _authorizationService.ListUserPermissionsAsync(user, cancellationToken);

        return new CurrentUserDto(user.Id, user.Email.Value, roles, permissions);
    }

    public Guid? ValidateToken(ValidateTokenQuery query)
    {
        return _jwtTokenGenerator.Validate(query.AccessToken);
    }

    private async Task<AuthenticationResult> IssueAuthenticationResultAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var roles = await _authorizationService.ListUserRolesAsync(user, cancellationToken);
        var permissions = await _authorizationService.ListUserPermissionsAsync(user, cancellationToken);
        var accessToken = _jwtTokenGenerator.Generate(user, roles);
        var utcNow = DateTime.UtcNow;
        var refreshTokenExpiresAtUtc = utcNow.AddDays(7);
        var refreshToken = RefreshToken.Issue(
            user.Id,
            GenerateRefreshTokenValue(),
            refreshTokenExpiresAtUtc,
            utcNow);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            user.Id,
            user.Email.Value,
            accessToken.AccessToken,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc,
            roles,
            permissions);
    }

    private static string GenerateRefreshTokenValue()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static void EnsurePasswordIsNotEmpty(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }
    }
}
