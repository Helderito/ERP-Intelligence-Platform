using ERP.Application.Identity.Abstractions;
using ERP.Application.Identity.Commands;
using ERP.Application.Identity.Models;
using ERP.Application.Identity.Queries;
using ERP.Domain.Identity;

namespace ERP.Application.Identity.Services;

public sealed class AuthorizationService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;

    public AuthorizationService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
    }

    public async Task<RoleDto> CreateRoleAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingRole = await _roleRepository.GetByNameAsync(command.Name, cancellationToken);

        if (existingRole is not null)
        {
            throw new InvalidOperationException("A role with this name already exists.");
        }

        var role = Role.Create(command.Name, DateTime.UtcNow);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return ToRoleDto(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(
        UpdateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRoleOrThrowAsync(command.RoleId, cancellationToken);
        var existingRole = await _roleRepository.GetByNameAsync(command.Name, cancellationToken);

        if (existingRole is not null && existingRole.Id != role.Id)
        {
            throw new InvalidOperationException("A role with this name already exists.");
        }

        role.UpdateName(command.Name, DateTime.UtcNow);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return ToRoleDto(role);
    }

    public async Task DeactivateRoleAsync(
        DeactivateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRoleOrThrowAsync(command.RoleId, cancellationToken);

        role.Deactivate(DateTime.UtcNow);
        await _roleRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<RoleDto> AssignPermissionsToRoleAsync(
        AssignPermissionsToRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await GetRoleOrThrowAsync(command.RoleId, cancellationToken);
        var permissions = await _permissionRepository.GetByIdsAsync(
            command.PermissionIds.Distinct().ToArray(),
            cancellationToken);

        if (permissions.Count != command.PermissionIds.Distinct().Count())
        {
            throw new InvalidOperationException("One or more permissions do not exist.");
        }

        foreach (var permission in permissions)
        {
            role.AssignPermission(permission.Id, DateTime.UtcNow);
        }

        await _roleRepository.SaveChangesAsync(cancellationToken);

        return ToRoleDto(role);
    }

    public async Task AssignRolesToUserAsync(
        AssignRolesToUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        var roles = await _roleRepository.GetByIdsAsync(command.RoleIds.Distinct().ToArray(), cancellationToken);

        if (roles.Count != command.RoleIds.Distinct().Count())
        {
            throw new InvalidOperationException("One or more roles do not exist.");
        }

        foreach (var role in roles.Where(role => role.IsActive))
        {
            user.AssignRole(role.Id, DateTime.UtcNow);
        }

        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(
        GetRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.ListAsync(query.IncludeInactive, cancellationToken);

        return roles.Select(ToRoleDto).ToArray();
    }

    public async Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(
        GetPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.ListAsync(cancellationToken);

        return permissions.Select(ToPermissionDto).ToArray();
    }

    public async Task<IReadOnlyCollection<string>> ListUserPermissionsAsync(
        ListUserPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        return await ListUserPermissionsAsync(user, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> ListUserPermissionsAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetByIdsAsync(user.RoleIds.ToArray(), cancellationToken);
        var permissionIds = roles
            .Where(role => role.IsActive)
            .SelectMany(role => role.PermissionIds)
            .Distinct()
            .ToArray();
        var permissions = await _permissionRepository.GetByIdsAsync(permissionIds, cancellationToken);

        return permissions
            .Select(permission => permission.Code)
            .OrderBy(code => code, StringComparer.Ordinal)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<string>> ListUserRolesAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetByIdsAsync(user.RoleIds.ToArray(), cancellationToken);

        return roles
            .Where(role => role.IsActive)
            .Select(role => role.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private async Task<Role> GetRoleOrThrowAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

        if (role is null)
        {
            throw new InvalidOperationException("Role does not exist.");
        }

        return role;
    }

    private static RoleDto ToRoleDto(Role role)
    {
        return new RoleDto(role.Id, role.Name, role.IsActive, role.PermissionIds.ToArray());
    }

    private static PermissionDto ToPermissionDto(Permission permission)
    {
        return new PermissionDto(permission.Id, permission.Code, permission.Description);
    }
}
