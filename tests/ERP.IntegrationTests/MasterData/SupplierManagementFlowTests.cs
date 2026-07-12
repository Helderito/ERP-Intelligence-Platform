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
public sealed class SupplierManagementFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public SupplierManagementFlowTests(AuthenticationWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SupplierManagementFlow_ShouldCreateGetSearchUpdateAndDeactivateSupplier()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";
        var adminEmail = $"supplier-admin-{Guid.NewGuid():N}@example.com";
        var supplierCode = $"SUP-{Guid.NewGuid():N}"[..16];
        var expectedSupplierCode = supplierCode.ToUpperInvariant();

        var admin = await RegisterAsync(client, adminEmail, password);
        await GrantAdministratorRoleAsync(admin.UserId);
        var adminLogin = await LoginAsync(client, adminEmail, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminLogin.AccessToken);

        var createResponse = await client.PostAsJsonAsync(
            "/suppliers",
            new CreateSupplierRequest(
                supplierCode,
                "Sample Supplier",
                [new SupplierContactRequest("Ana Silva", "ana@example.com", "+244 900 000 000")],
                [new SupplierAddressRequest("Rua Principal", null, "Luanda", "1000", "Angola")]));
        createResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdSupplier = await createResponse.Content.ReadFromJsonAsync<SupplierResponse>();

        Assert.NotNull(createdSupplier);
        Assert.Equal(expectedSupplierCode, createdSupplier.Code);
        Assert.Single(createdSupplier.Contacts);
        Assert.Single(createdSupplier.Addresses);

        var getResponse = await client.GetAsync($"/suppliers/{createdSupplier.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedSupplier = await getResponse.Content.ReadFromJsonAsync<SupplierResponse>();

        Assert.Equal(createdSupplier.Id, fetchedSupplier?.Id);

        var searchResponse = await client.GetAsync($"/suppliers?search={expectedSupplierCode}&page=1&pageSize=20");
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResultResponse<SupplierListItemResponse>>();

        Assert.Equal(1, searchResult?.TotalRecords);
        Assert.Contains(searchResult?.Items ?? [], supplier => supplier.Id == createdSupplier.Id);
        var listedSupplier = Assert.Single(searchResult?.Items ?? []);
        Assert.Equal(expectedSupplierCode, listedSupplier.Code);
        Assert.Equal("Sample Supplier", listedSupplier.Name);
        Assert.True(listedSupplier.IsActive);

        var updateResponse = await client.PutAsJsonAsync(
            $"/suppliers/{createdSupplier.Id}",
            new UpdateSupplierRequest(
                "Updated Supplier",
                [new SupplierContactRequest("Carlos Lopes", "carlos@example.com", "+351 900 000 000")],
                [new SupplierAddressRequest("Avenida Nova", "2A", "Lisboa", "1200-001", "Portugal")]));
        updateResponse.EnsureSuccessStatusCode();
        var updatedSupplier = await updateResponse.Content.ReadFromJsonAsync<SupplierResponse>();

        Assert.Equal("Updated Supplier", updatedSupplier?.Name);
        Assert.Equal(expectedSupplierCode, updatedSupplier?.Code);
        Assert.Equal("Carlos Lopes", updatedSupplier?.Contacts.Single().Name);
        Assert.Equal("Lisboa", updatedSupplier?.Addresses.Single().City);

        var deactivateResponse = await client.DeleteAsync($"/suppliers/{createdSupplier.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var deactivatedSupplierResponse = await client.GetAsync($"/suppliers/{createdSupplier.Id}");
        deactivatedSupplierResponse.EnsureSuccessStatusCode();
        var deactivatedSupplier = await deactivatedSupplierResponse.Content.ReadFromJsonAsync<SupplierResponse>();

        Assert.False(deactivatedSupplier?.IsActive);
    }

    [Fact]
    public async Task SupplierEndpoints_ShouldReturnForbidden_WhenUserLacksSupplierPermission()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";

        await RegisterAsync(client, $"bootstrap-{Guid.NewGuid():N}@example.com", password);
        var limitedUser = await RegisterAsync(client, $"limited-{Guid.NewGuid():N}@example.com", password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limitedUser.AccessToken);

        var response = await client.GetAsync("/suppliers");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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

    private async Task GrantAdministratorRoleAsync(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users
            .Include("_userRoles")
            .FirstAsync(item => item.Id == userId);

        user.AssignRole(IdentitySeed.AdministratorRoleId, DateTime.UtcNow);
        await dbContext.SaveChangesAsync();
    }
}
