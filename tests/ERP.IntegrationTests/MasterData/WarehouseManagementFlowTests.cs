using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.MasterData;
using ERP.Domain.Identity;
using ERP.Infrastructure.Persistence;
using ERP.IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.IntegrationTests.MasterData;

[Collection(AuthenticationTestCollection.Name)]
public sealed class WarehouseManagementFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public WarehouseManagementFlowTests(AuthenticationWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task WarehouseManagementFlow_ShouldCreateGetSearchUpdateAndDeactivateWarehouse()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";
        var adminEmail = $"warehouse-admin-{Guid.NewGuid():N}@example.com";
        var warehouseCode = $"WH-{Guid.NewGuid():N}"[..16];
        var admin = await RegisterAsync(client, adminEmail, password);
        await GrantAdministratorRoleAsync(admin.UserId);
        var login = await LoginAsync(client, adminEmail, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var typesResponse = await client.GetAsync("/warehouse-types");
        typesResponse.EnsureSuccessStatusCode();
        var types = await typesResponse.Content.ReadFromJsonAsync<WarehouseTypeResponse[]>();
        Assert.Equal(3, types?.Length);
        var mainType = Assert.Single(types ?? [], type => type.Code == "MAIN");
        var transitType = Assert.Single(types ?? [], type => type.Code == "TRANSIT");

        var createResponse = await client.PostAsJsonAsync(
            "/warehouses",
            new CreateWarehouseRequest(warehouseCode, "Main Warehouse", mainType.Id));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(created);
        Assert.Equal(mainType.Name, created.WarehouseTypeName);

        var getResponse = await client.GetAsync($"/warehouses/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.Equal(created.Id, fetched?.Id);
        Assert.Equal(mainType.Name, fetched?.WarehouseTypeName);

        var searchResponse = await client.GetAsync($"/warehouses?search={warehouseCode}&page=1&pageSize=20");
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResultResponse<WarehouseListItemResponse>>();
        var listed = Assert.Single(searchResult?.Items ?? []);
        Assert.Equal(created.Id, listed.Id);

        var updateResponse = await client.PutAsJsonAsync(
            $"/warehouses/{created.Id}",
            new UpdateWarehouseRequest("Transit Warehouse", transitType.Id));
        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.Equal("Transit Warehouse", updated?.Name);
        Assert.Equal(transitType.Name, updated?.WarehouseTypeName);
        Assert.Equal(created.Code, updated?.Code);

        var deactivateResponse = await client.DeleteAsync($"/warehouses/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var deactivatedResponse = await client.GetAsync($"/warehouses/{created.Id}");
        deactivatedResponse.EnsureSuccessStatusCode();
        var deactivated = await deactivatedResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.False(deactivated?.IsActive);
    }

    [Fact]
    public async Task WarehouseEndpoints_ShouldReturnUnauthorizedOrForbidden_WhenPermissionIsMissing()
    {
        using var client = _factory.CreateClient();
        var unauthenticatedResponse = await client.GetAsync("/warehouses");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedResponse.StatusCode);

        const string password = "CorrectHorseBatteryStaple!";
        await RegisterAsync(client, $"bootstrap-{Guid.NewGuid():N}@example.com", password);
        var limitedUser = await RegisterAsync(client, $"limited-{Guid.NewGuid():N}@example.com", password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limitedUser.AccessToken);

        var forbiddenResponse = await client.GetAsync("/warehouses");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
    }

    private static async Task<AuthenticationResponse> RegisterAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/auth/register", new RegisterUserRequest(email, password));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    private static async Task<AuthenticationResponse> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new LoginRequest(email, password));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>())!;
    }

    private async Task GrantAdministratorRoleAsync(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.Include("_userRoles").FirstAsync(item => item.Id == userId);
        user.AssignRole(IdentitySeed.AdministratorRoleId, DateTime.UtcNow);
        await dbContext.SaveChangesAsync();
    }
}
