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
public sealed class ManagedReferenceDataFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public ManagedReferenceDataFlowTests(AuthenticationWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task ManagedReferenceDataFlow_ShouldPreserveProductReferencesAndManageAllFamilies()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";
        var email = $"reference-admin-{Guid.NewGuid():N}@example.com";
        var admin = await RegisterAsync(client, email, password);
        await GrantAdministratorRoleAsync(admin.UserId);
        var login = await LoginAsync(client, email, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var seededCategories = await client.GetFromJsonAsync<CategoryResponse[]>("/categories");
        var seededUnits = await client.GetFromJsonAsync<UnitOfMeasureResponse[]>("/units-of-measure");
        Assert.All(seededCategories ?? [], item => Assert.True(item.IsActive));
        Assert.All(seededUnits ?? [], item => Assert.True(item.IsActive));

        var categoryCode = $"CAT-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        var categoryResponse = await client.PostAsJsonAsync(
            "/categories", new CreateReferenceDataRequest(categoryCode, "Managed Category"));
        Assert.Equal(HttpStatusCode.Created, categoryResponse.StatusCode);
        var category = (await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>())!;

        var unitCode = $"UOM-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        var unitResponse = await client.PostAsJsonAsync(
            "/units-of-measure", new CreateReferenceDataRequest(unitCode, "Managed Unit"));
        Assert.Equal(HttpStatusCode.Created, unitResponse.StatusCode);
        var unit = (await unitResponse.Content.ReadFromJsonAsync<UnitOfMeasureResponse>())!;

        var productResponse = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest($"SKU-{Guid.NewGuid():N}"[..16], "Reference Product", category.Id, unit.Id));
        productResponse.EnsureSuccessStatusCode();
        var product = (await productResponse.Content.ReadFromJsonAsync<ProductResponse>())!;

        var updateCategoryResponse = await client.PutAsJsonAsync(
            $"/categories/{category.Id}", new UpdateReferenceDataRequest("Updated Category"));
        updateCategoryResponse.EnsureSuccessStatusCode();
        var updatedCategory = await updateCategoryResponse.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.Equal(category.Code, updatedCategory?.Code);

        var updateUnitResponse = await client.PutAsJsonAsync(
            $"/units-of-measure/{unit.Id}", new UpdateReferenceDataRequest("Updated Unit"));
        updateUnitResponse.EnsureSuccessStatusCode();
        var updatedUnit = await updateUnitResponse.Content.ReadFromJsonAsync<UnitOfMeasureResponse>();
        Assert.Equal(unit.Code, updatedUnit?.Code);

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/categories/{category.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/units-of-measure/{unit.Id}")).StatusCode);

        var activeCategories = await client.GetFromJsonAsync<CategoryResponse[]>("/categories");
        var activeUnits = await client.GetFromJsonAsync<UnitOfMeasureResponse[]>("/units-of-measure");
        Assert.DoesNotContain(activeCategories ?? [], item => item.Id == category.Id);
        Assert.DoesNotContain(activeUnits ?? [], item => item.Id == unit.Id);

        var managedCategories = await client.GetFromJsonAsync<CategoryResponse[]>("/categories?includeInactive=true");
        var managedUnits = await client.GetFromJsonAsync<UnitOfMeasureResponse[]>("/units-of-measure?includeInactive=true");
        Assert.Contains(managedCategories ?? [], item => item.Id == category.Id && !item.IsActive);
        Assert.Contains(managedUnits ?? [], item => item.Id == unit.Id && !item.IsActive);

        var persistedProduct = await client.GetFromJsonAsync<ProductResponse>($"/products/{product.Id}");
        Assert.Equal(category.Id, persistedProduct?.CategoryId);
        Assert.Equal(unit.Id, persistedProduct?.UnitOfMeasureId);

        var taxCodeValue = $"TAX-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        var createTaxResponse = await client.PostAsJsonAsync(
            "/tax-codes", new CreateTaxCodeRequest(taxCodeValue, "General Tax", 14));
        Assert.Equal(HttpStatusCode.Created, createTaxResponse.StatusCode);
        var taxCode = (await createTaxResponse.Content.ReadFromJsonAsync<TaxCodeResponse>())!;

        var taxDetail = await client.GetFromJsonAsync<TaxCodeResponse>($"/tax-codes/{taxCode.Id}");
        Assert.Equal(14, taxDetail?.Rate);

        var updateTaxResponse = await client.PutAsJsonAsync(
            $"/tax-codes/{taxCode.Id}", new UpdateTaxCodeRequest("Updated Tax", 15.5m));
        updateTaxResponse.EnsureSuccessStatusCode();
        var updatedTax = await updateTaxResponse.Content.ReadFromJsonAsync<TaxCodeResponse>();
        Assert.Equal(taxCode.Code, updatedTax?.Code);
        Assert.Equal(15.5m, updatedTax?.Rate);

        var invalidTaxResponse = await client.PostAsJsonAsync(
            "/tax-codes", new CreateTaxCodeRequest($"BAD-{Guid.NewGuid():N}"[..16], "Invalid", 100.01m));
        Assert.Equal(HttpStatusCode.BadRequest, invalidTaxResponse.StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/tax-codes/{taxCode.Id}")).StatusCode);
        var activeTaxCodes = await client.GetFromJsonAsync<TaxCodeResponse[]>("/tax-codes");
        var managedTaxCodes = await client.GetFromJsonAsync<TaxCodeResponse[]>("/tax-codes?includeInactive=true");
        Assert.DoesNotContain(activeTaxCodes ?? [], item => item.Id == taxCode.Id);
        Assert.Contains(managedTaxCodes ?? [], item => item.Id == taxCode.Id && !item.IsActive);
    }

    [Fact]
    public async Task ReferenceWrites_ShouldRequirePermission_WhileReadsRequireAuthentication()
    {
        using var client = _factory.CreateClient();
        var unauthenticatedRead = await client.GetAsync("/categories");
        var unauthenticatedWrite = await client.PostAsJsonAsync(
            "/categories", new CreateReferenceDataRequest("NOAUTH", "No Auth"));
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedRead.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedWrite.StatusCode);

        const string password = "CorrectHorseBatteryStaple!";
        await RegisterAsync(client, $"bootstrap-{Guid.NewGuid():N}@example.com", password);
        var limited = await RegisterAsync(client, $"limited-{Guid.NewGuid():N}@example.com", password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limited.AccessToken);

        var authenticatedRead = await client.GetAsync("/categories");
        var forbiddenWrite = await client.PostAsJsonAsync(
            "/categories", new CreateReferenceDataRequest("FORBIDDEN", "Forbidden"));
        Assert.Equal(HttpStatusCode.OK, authenticatedRead.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenWrite.StatusCode);
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
