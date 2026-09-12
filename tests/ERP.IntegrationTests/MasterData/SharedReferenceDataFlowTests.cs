using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ERP.Api.Contracts.Authentication;
using ERP.Api.Contracts.MasterData;
using ERP.IntegrationTests.Support;

namespace ERP.IntegrationTests.MasterData;

[Collection(AuthenticationTestCollection.Name)]
public sealed class SharedReferenceDataFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public SharedReferenceDataFlowTests(AuthenticationWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task ReferenceEndpoints_ShouldReturnOrderedSeededLists_ForAuthenticatedUser()
    {
        using var client = _factory.CreateClient();
        var authentication = await RegisterAsync(
            client,
            $"reference-reader-{Guid.NewGuid():N}@example.com",
            "CorrectHorseBatteryStaple!");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authentication.AccessToken);

        var countries = await client.GetFromJsonAsync<CountryResponse[]>("/countries");
        var currencies = await client.GetFromJsonAsync<CurrencyResponse[]>("/currencies");
        var paymentTerms = await client.GetFromJsonAsync<PaymentTermResponse[]>("/payment-terms");

        Assert.Equal(25, countries?.Length);
        Assert.Contains(countries ?? [], country => country.Code == "PT" && country.Name == "Portugal");
        Assert.Contains(countries ?? [], country => country.Code == "AO" && country.Name == "Angola");
        Assert.Equal((countries ?? []).OrderBy(country => country.Code).Select(country => country.Code),
            (countries ?? []).Select(country => country.Code));

        Assert.Equal(15, currencies?.Length);
        Assert.Contains(currencies ?? [], currency => currency.Code == "EUR");
        Assert.Contains(currencies ?? [], currency => currency.Code == "USD");
        Assert.Contains(currencies ?? [], currency => currency.Code == "AOA");
        Assert.Contains(currencies ?? [], currency => currency.Code == "BRL");
        Assert.Contains(currencies ?? [], currency => currency.Code == "GBP");
        Assert.Equal((currencies ?? []).OrderBy(currency => currency.Code).Select(currency => currency.Code),
            (currencies ?? []).Select(currency => currency.Code));

        Assert.Equal(5, paymentTerms?.Length);
        Assert.Collection(
            paymentTerms ?? [],
            term => Assert.Equal(("NET0", 0), (term.Code, term.NetDays)),
            term => Assert.Equal(("NET15", 15), (term.Code, term.NetDays)),
            term => Assert.Equal(("NET30", 30), (term.Code, term.NetDays)),
            term => Assert.Equal(("NET60", 60), (term.Code, term.NetDays)),
            term => Assert.Equal(("NET90", 90), (term.Code, term.NetDays)));
    }

    [Fact]
    public async Task ReferenceEndpoints_ShouldRequireAuthentication()
    {
        using var client = _factory.CreateClient();

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/countries")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/currencies")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/payment-terms")).StatusCode);
    }

    [Fact]
    public async Task ReferenceEndpoints_ShouldNotExposeWriteOperations()
    {
        using var client = _factory.CreateClient();
        var authentication = await RegisterAsync(
            client,
            $"reference-write-check-{Guid.NewGuid():N}@example.com",
            "CorrectHorseBatteryStaple!");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authentication.AccessToken);

        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.PostAsJsonAsync("/countries", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.PostAsJsonAsync("/currencies", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.PostAsJsonAsync("/payment-terms", new { })).StatusCode);
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
}
