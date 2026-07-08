namespace ERP.IntegrationTests.Support;

[CollectionDefinition(Name)]
public sealed class AuthenticationTestCollection : ICollectionFixture<AuthenticationWebApplicationFactory>
{
    public const string Name = "Authentication integration tests";
}
