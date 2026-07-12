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
public sealed class CustomerManagementFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public CustomerManagementFlowTests(AuthenticationWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CustomerManagementFlow_ShouldCreateGetSearchUpdateAndDeactivateCustomer()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";
        var adminEmail = $"customer-admin-{Guid.NewGuid():N}@example.com";
        var customerCode = $"CUS-{Guid.NewGuid():N}"[..16];
        var expectedCustomerCode = customerCode.ToUpperInvariant();

        var admin = await RegisterAsync(client, adminEmail, password);
        await GrantAdministratorRoleAsync(admin.UserId);
        var adminLogin = await LoginAsync(client, adminEmail, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminLogin.AccessToken);

        var createResponse = await client.PostAsJsonAsync(
            "/customers",
            new CreateCustomerRequest(
                customerCode,
                "Sample Customer",
                [new CustomerContactRequest("Ana Silva", "ana@example.com", "+244 900 000 000")],
                [new CustomerAddressRequest("Rua Principal", null, "Luanda", "1000", "Angola")]));
        createResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(createdCustomer);
        Assert.Equal(expectedCustomerCode, createdCustomer.Code);
        Assert.Single(createdCustomer.Contacts);
        Assert.Single(createdCustomer.Addresses);

        var getResponse = await client.GetAsync($"/customers/{createdCustomer.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedCustomer = await getResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        Assert.Equal(createdCustomer.Id, fetchedCustomer?.Id);

        var searchResponse = await client.GetAsync($"/customers?search={expectedCustomerCode}&page=1&pageSize=20");
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResultResponse<CustomerListItemResponse>>();

        Assert.Equal(1, searchResult?.TotalRecords);
        Assert.Contains(searchResult?.Items ?? [], customer => customer.Id == createdCustomer.Id);
        var listedCustomer = Assert.Single(searchResult?.Items ?? []);
        Assert.Equal(expectedCustomerCode, listedCustomer.Code);
        Assert.Equal("Sample Customer", listedCustomer.Name);
        Assert.True(listedCustomer.IsActive);

        var updateResponse = await client.PutAsJsonAsync(
            $"/customers/{createdCustomer.Id}",
            new UpdateCustomerRequest(
                "Updated Customer",
                [new CustomerContactRequest("Carlos Lopes", "carlos@example.com", "+351 900 000 000")],
                [new CustomerAddressRequest("Avenida Nova", "2A", "Lisboa", "1200-001", "Portugal")]));
        updateResponse.EnsureSuccessStatusCode();
        var updatedCustomer = await updateResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        Assert.Equal("Updated Customer", updatedCustomer?.Name);
        Assert.Equal(expectedCustomerCode, updatedCustomer?.Code);
        Assert.Equal("Carlos Lopes", updatedCustomer?.Contacts.Single().Name);
        Assert.Equal("Lisboa", updatedCustomer?.Addresses.Single().City);

        var deactivateResponse = await client.DeleteAsync($"/customers/{createdCustomer.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var deactivatedCustomerResponse = await client.GetAsync($"/customers/{createdCustomer.Id}");
        deactivatedCustomerResponse.EnsureSuccessStatusCode();
        var deactivatedCustomer = await deactivatedCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        Assert.False(deactivatedCustomer?.IsActive);
    }

    [Fact]
    public async Task CustomerEndpoints_ShouldReturnForbidden_WhenUserLacksCustomerPermission()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";

        await RegisterAsync(client, $"bootstrap-{Guid.NewGuid():N}@example.com", password);
        var limitedUser = await RegisterAsync(client, $"limited-{Guid.NewGuid():N}@example.com", password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limitedUser.AccessToken);

        var response = await client.GetAsync("/customers");

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
