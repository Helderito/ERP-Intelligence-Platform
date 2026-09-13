using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.MasterData;
using ERP.Api.Contracts.Tenancy;
using ERP.Domain.Identity;
using ERP.Domain.Tenancy;
using ERP.Infrastructure.Persistence;
using ERP.IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.IntegrationTests.Tenancy;

[Collection(AuthenticationTestCollection.Name)]
public sealed class TenancyIsolationFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public TenancyIsolationFlowTests(AuthenticationWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task TenancyFlow_ShouldBootstrapManageAndIsolateCompanyOwnedData()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";
        var firstEmail = $"tenant-a-{Guid.NewGuid():N}@example.com";
        var secondEmail = $"tenant-b-{Guid.NewGuid():N}@example.com";

        var unauthenticatedResponse = await client.GetAsync("/companies");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedResponse.StatusCode);

        var firstRegistration = await RegisterAsync(client, firstEmail, password);
        Assert.Equal(TenancySeed.DefaultCompanyId, firstRegistration.CompanyId);
        Assert.Equal(
            TenancySeed.DefaultCompanyId.ToString(),
            new JwtSecurityTokenHandler().ReadJwtToken(firstRegistration.AccessToken)
                .Claims.Single(claim => claim.Type == "companyId").Value);

        await GrantAdministratorRoleAsync(firstRegistration.UserId);
        var firstLogin = await LoginAsync(client, firstEmail, password);
        client.DefaultRequestHeaders.Authorization = Bearer(firstLogin.AccessToken);

        var defaultCompanyResponse = await client.GetAsync($"/companies/{TenancySeed.DefaultCompanyId}");
        defaultCompanyResponse.EnsureSuccessStatusCode();
        var defaultCompany = await defaultCompanyResponse.Content.ReadFromJsonAsync<CompanyResponse>();
        Assert.Contains(defaultCompany?.Establishments ?? [], item => item.Code == "MAIN");

        var secondCompanyResponse = await client.PostAsJsonAsync(
            "/companies",
            new CreateCompanyRequest("Second Company", "MAIN", "Head Office", "001"));
        Assert.Equal(HttpStatusCode.Created, secondCompanyResponse.StatusCode);
        var secondCompany = (await secondCompanyResponse.Content.ReadFromJsonAsync<CompanyResponse>())!;

        var profileResponse = await client.PutAsJsonAsync(
            $"/companies/{secondCompany.Id}/fiscal-profile",
            new UpdateCompanyFiscalProfileRequest("AO-123456", "General", "Lobito, Angola"));
        profileResponse.EnsureSuccessStatusCode();

        var secondRegistration = await RegisterAsync(client, secondEmail, password);
        client.DefaultRequestHeaders.Authorization = Bearer(secondRegistration.AccessToken);
        var unauthorizedCompanyManagement = await client.GetAsync("/companies");
        Assert.Equal(HttpStatusCode.Forbidden, unauthorizedCompanyManagement.StatusCode);

        await MoveUserToCompanyAndGrantAdministratorAsync(secondRegistration.UserId, secondCompany.Id);
        var secondLogin = await LoginAsync(client, secondEmail, password);
        Assert.Equal(secondCompany.Id, secondLogin.CompanyId);
        client.DefaultRequestHeaders.Authorization = Bearer(secondLogin.AccessToken);

        var customerCode = $"TEN-{Guid.NewGuid():N}"[..16];
        var createCustomerResponse = await client.PostAsJsonAsync(
            "/customers",
            new CreateCustomerRequest(customerCode, "Second Company Customer", [], []));
        Assert.Equal(HttpStatusCode.Created, createCustomerResponse.StatusCode);
        var secondCompanyCustomer = (await createCustomerResponse.Content.ReadFromJsonAsync<CustomerResponse>())!;

        var globalCountriesResponse = await client.GetAsync("/countries");
        globalCountriesResponse.EnsureSuccessStatusCode();
        Assert.NotEmpty((await globalCountriesResponse.Content.ReadFromJsonAsync<CountryResponse[]>()) ?? []);

        client.DefaultRequestHeaders.Authorization = Bearer(firstLogin.AccessToken);
        var sameCodeInFirstCompany = await client.PostAsJsonAsync(
            "/customers",
            new CreateCustomerRequest(customerCode, "First Company Customer", [], []));
        Assert.Equal(HttpStatusCode.Created, sameCodeInFirstCompany.StatusCode);

        var crossCompanyRead = await client.GetAsync($"/customers/{secondCompanyCustomer.Id}");
        Assert.Equal(HttpStatusCode.NotFound, crossCompanyRead.StatusCode);

        var crossCompanyWrite = await client.PutAsJsonAsync(
            $"/customers/{secondCompanyCustomer.Id}",
            new UpdateCustomerRequest("Forbidden Update", [], []));
        Assert.Equal(HttpStatusCode.NotFound, crossCompanyWrite.StatusCode);

        var searchResponse = await client.GetAsync($"/customers?search={customerCode}&page=1&pageSize=20");
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResultResponse<CustomerListItemResponse>>();
        var visibleCustomer = Assert.Single(searchResult?.Items ?? []);
        Assert.Equal("First Company Customer", visibleCustomer.Name);
        Assert.NotEqual(secondCompanyCustomer.Id, visibleCustomer.Id);
    }

    private static AuthenticationHeaderValue Bearer(string token) => new("Bearer", token);

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
        var user = await dbContext.Users.Include("_userRoles").SingleAsync(item => item.Id == userId);
        user.AssignRole(IdentitySeed.AdministratorRoleId, DateTime.UtcNow);
        await dbContext.SaveChangesAsync();
    }

    private async Task MoveUserToCompanyAndGrantAdministratorAsync(Guid userId, Guid companyId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.Include("_userRoles").SingleAsync(item => item.Id == userId);
        user.AssignRole(IdentitySeed.AdministratorRoleId, DateTime.UtcNow);
        await dbContext.UserCompanies
            .Where(item => item.UserId == userId)
            .ExecuteUpdateAsync(update => update.SetProperty(item => item.CompanyId, companyId));
        await dbContext.SaveChangesAsync();
    }
}
