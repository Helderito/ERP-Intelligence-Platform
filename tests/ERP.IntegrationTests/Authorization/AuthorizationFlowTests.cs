using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.Authorization;
using ERP.Domain.Identity;
using ERP.Infrastructure.Persistence;
using ERP.IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.IntegrationTests.Authorization;

[Collection(AuthenticationTestCollection.Name)]
public sealed class AuthorizationFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public AuthorizationFlowTests(AuthenticationWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthorizationFlow_ShouldValidatePermissionsAndReturnForbidden_WhenUserLacksPermission()
    {
        using var client = _factory.CreateClient();
        var adminEmail = $"admin-{Guid.NewGuid():N}@example.com";
        var limitedEmail = $"limited-{Guid.NewGuid():N}@example.com";
        const string password = "CorrectHorseBatteryStaple!";

        var admin = await RegisterAsync(client, adminEmail, password);
        var limitedUser = await RegisterAsync(client, limitedEmail, password);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limitedUser.AccessToken);
        var forbiddenResponse = await client.GetAsync("/roles");

        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);

        await GrantAdministrativePermissionsAsync(admin.UserId);

        var adminLogin = await LoginAsync(client, adminEmail, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminLogin.AccessToken);

        var permissionsResponse = await client.GetAsync("/permissions");
        permissionsResponse.EnsureSuccessStatusCode();
        var permissions = await permissionsResponse.Content.ReadFromJsonAsync<PermissionResponse[]>();
        var rolesManagePermission = Assert.Single(
            permissions ?? [],
            permission => permission.Code == PermissionCodes.RolesManage);

        var createRoleResponse = await client.PostAsJsonAsync("/roles", new CreateRoleRequest("Managers"));
        createRoleResponse.EnsureSuccessStatusCode();
        var managerRole = await createRoleResponse.Content.ReadFromJsonAsync<RoleResponse>();

        Assert.NotNull(managerRole);

        var assignPermissionResponse = await client.PostAsJsonAsync(
            $"/roles/{managerRole.Id}/permissions",
            new AssignPermissionsToRoleRequest([rolesManagePermission.Id]));
        assignPermissionResponse.EnsureSuccessStatusCode();

        var assignRoleResponse = await client.PostAsJsonAsync(
            $"/users/{limitedUser.UserId}/roles",
            new AssignRolesToUserRequest([managerRole.Id]));

        Assert.Equal(HttpStatusCode.NoContent, assignRoleResponse.StatusCode);

        var limitedLogin = await LoginAsync(client, limitedEmail, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limitedLogin.AccessToken);

        var authorizedResponse = await client.GetAsync("/roles");
        authorizedResponse.EnsureSuccessStatusCode();
    }

    private static async Task<AuthenticationResponse> RegisterAsync(
        HttpClient client,
        string email,
        string password)
    {
        var response = await client.PostAsJsonAsync("/auth/register", new RegisterUserRequest(email, password));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    private static async Task<AuthenticationResponse> LoginAsync(
        HttpClient client,
        string email,
        string password)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new LoginRequest(email, password));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    private async Task GrantAdministrativePermissionsAsync(Guid adminUserId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var adminUser = await dbContext.Users
            .Include("_userRoles")
            .FirstAsync(user => user.Id == adminUserId);
        var permissions = await dbContext.Permissions
            .Where(permission =>
                permission.Code == PermissionCodes.RolesManage ||
                permission.Code == PermissionCodes.UsersManage)
            .ToArrayAsync();
        var role = Role.Create($"Administrators-{Guid.NewGuid():N}", DateTime.UtcNow);

        foreach (var permission in permissions)
        {
            role.AssignPermission(permission.Id, DateTime.UtcNow);
        }

        await dbContext.Roles.AddAsync(role);
        adminUser.AssignRole(role.Id, DateTime.UtcNow);
        await dbContext.SaveChangesAsync();
    }
}
