using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ERP.Api.Contracts.Authentication;
using ERP.IntegrationTests.Support;

namespace ERP.IntegrationTests.Authentication;

[Collection(AuthenticationTestCollection.Name)]
public sealed class AuthenticationFlowTests
{
    private readonly AuthenticationWebApplicationFactory _factory;

    public AuthenticationFlowTests(AuthenticationWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthenticationFlow_ShouldRegisterLoginAccessRefreshAndLogout()
    {
        using var client = _factory.CreateClient();
        var email = $"user-{Guid.NewGuid():N}@example.com";
        var password = "CorrectHorseBatteryStaple!";

        var registerResponse = await client.PostAsJsonAsync("/auth/register", new RegisterUserRequest(email, password));
        registerResponse.EnsureSuccessStatusCode();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        Assert.NotNull(registerResult);
        Assert.Equal(email, registerResult.Email);
        Assert.False(string.IsNullOrWhiteSpace(registerResult.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(registerResult.RefreshToken));

        var invalidLoginResponse = await client.PostAsJsonAsync("/auth/login", new LoginRequest(email, "wrong-password"));
        Assert.Equal(HttpStatusCode.Unauthorized, invalidLoginResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/auth/login", new LoginRequest(email, password));
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        Assert.NotNull(loginResult);

        using var unauthenticatedMeResponse = await client.GetAsync("/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedMeResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        var meResponse = await client.GetAsync("/auth/me");
        meResponse.EnsureSuccessStatusCode();
        var currentUser = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.NotNull(currentUser);
        Assert.Equal(email, currentUser.Email);

        var refreshResponse = await client.PostAsJsonAsync(
            "/auth/refresh",
            new RefreshTokenRequest(loginResult.RefreshToken));
        refreshResponse.EnsureSuccessStatusCode();
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

        Assert.NotNull(refreshResult);
        Assert.NotEqual(loginResult.RefreshToken, refreshResult.RefreshToken);

        var logoutResponse = await client.PostAsJsonAsync(
            "/auth/logout",
            new LogoutRequest(refreshResult.RefreshToken));
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        var revokedRefreshResponse = await client.PostAsJsonAsync(
            "/auth/refresh",
            new RefreshTokenRequest(refreshResult.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, revokedRefreshResponse.StatusCode);
    }
}
