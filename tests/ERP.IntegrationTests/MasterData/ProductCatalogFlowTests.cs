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
public sealed class ProductCatalogFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public ProductCatalogFlowTests(AuthenticationWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductCatalogFlow_ShouldCreateGetSearchUpdateAndDeactivateProduct()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";
        var adminEmail = $"catalog-admin-{Guid.NewGuid():N}@example.com";
        var productCode = $"SKU-{Guid.NewGuid():N}"[..16];
        var expectedProductCode = productCode.ToUpperInvariant();

        var admin = await RegisterAsync(client, adminEmail, password);
        await GrantAdministratorRoleAsync(admin.UserId);
        var adminLogin = await LoginAsync(client, adminEmail, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminLogin.AccessToken);

        var categoriesResponse = await client.GetAsync("/categories");
        categoriesResponse.EnsureSuccessStatusCode();
        var categories = await categoriesResponse.Content.ReadFromJsonAsync<CategoryResponse[]>();
        var category = Assert.Single(categories ?? []);

        var unitsResponse = await client.GetAsync("/units-of-measure");
        unitsResponse.EnsureSuccessStatusCode();
        var units = await unitsResponse.Content.ReadFromJsonAsync<UnitOfMeasureResponse[]>();
        var unit = Assert.Single(units ?? [], item => item.Code == "UNIT");
        var kilogram = Assert.Single(units ?? [], item => item.Code == "KG");

        var createResponse = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest(productCode, "Sample Product", category.Id, unit.Id));
        createResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(createdProduct);
        Assert.Equal(expectedProductCode, createdProduct.Code);

        var getResponse = await client.GetAsync($"/products/{createdProduct.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.Equal(createdProduct.Id, fetchedProduct?.Id);

        var searchResponse = await client.GetAsync($"/products?search={expectedProductCode}&page=1&pageSize=20");
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResultResponse<ProductListItemResponse>>();

        Assert.Equal(1, searchResult?.TotalRecords);
        Assert.Contains(searchResult?.Items ?? [], product => product.Id == createdProduct.Id);
        var listedProduct = Assert.Single(searchResult?.Items ?? []);
        Assert.Equal(expectedProductCode, listedProduct.Code);
        Assert.Equal("Sample Product", listedProduct.Name);
        Assert.True(listedProduct.IsActive);

        var updateResponse = await client.PutAsJsonAsync(
            $"/products/{createdProduct.Id}",
            new UpdateProductRequest("Updated Product", category.Id, kilogram.Id));
        updateResponse.EnsureSuccessStatusCode();
        var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.Equal("Updated Product", updatedProduct?.Name);
        Assert.Equal(expectedProductCode, updatedProduct?.Code);
        Assert.Equal(kilogram.Id, updatedProduct?.UnitOfMeasureId);

        var deactivateResponse = await client.DeleteAsync($"/products/{createdProduct.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var deactivatedProductResponse = await client.GetAsync($"/products/{createdProduct.Id}");
        deactivatedProductResponse.EnsureSuccessStatusCode();
        var deactivatedProduct = await deactivatedProductResponse.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.False(deactivatedProduct?.IsActive);
    }

    [Fact]
    public async Task ProductCatalogEndpoints_ShouldReturnForbidden_WhenUserLacksCatalogPermission()
    {
        using var client = _factory.CreateClient();
        const string password = "CorrectHorseBatteryStaple!";

        await RegisterAsync(client, $"bootstrap-{Guid.NewGuid():N}@example.com", password);
        var limitedUser = await RegisterAsync(client, $"limited-{Guid.NewGuid():N}@example.com", password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", limitedUser.AccessToken);

        var response = await client.GetAsync("/products");

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
