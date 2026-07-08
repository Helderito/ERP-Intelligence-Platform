using ERP.IntegrationTests.Support;

namespace ERP.IntegrationTests.Health;

[Collection(AuthenticationTestCollection.Name)]
public sealed class HealthEndpointTests
{
    private readonly AuthenticationWebApplicationFactory factory;

    public HealthEndpointTests(AuthenticationWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnSuccess()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
    }
}
