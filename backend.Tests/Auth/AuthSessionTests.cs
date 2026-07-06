using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using OnevoHr.Api.Data;
using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Auth;

public class AuthSessionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthSessionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, SeededTenant Seed)> CreateSeededClientAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);
        return (client, seed);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var (client, seed) = await CreateSeededClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = "wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_SetsSessionAndCsrfCookies()
    {
        var (client, seed) = await CreateSeededClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var setCookies = response.Headers.TryGetValues("Set-Cookie", out var cookies) ? cookies.ToList() : new List<string>();
        Assert.Contains(setCookies, c => c.StartsWith($"{CurrentUserMiddleware.TenantSessionCookie}=") && c.Contains("httponly", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(setCookies, c => c.StartsWith("onevo_csrf="));
    }

    [Fact]
    public async Task Login_ResponseBody_DoesNotContainTokenFields()
    {
        var (client, seed) = await CreateSeededClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });
        var body = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("accessToken", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("refreshToken", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("jwt", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Session_AfterLogin_ReturnsPermissionsModulesFeatures()
    {
        var (client, seed) = await CreateSeededClientAsync();
        await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });

        var response = await client.GetAsync("/api/v1/auth/session");
        var session = await response.Content.ReadFromJsonAsync<SessionDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(session!.User);
        Assert.Contains("widgets:read", session.Permissions);
        Assert.Contains("widgets:manage", session.Permissions);
        Assert.Contains("widgets", session.ActiveModules);
        Assert.Contains("widgets_pro", session.ActiveFeatures);
    }

    [Fact]
    public async Task Session_WithoutCookie_ReturnsNullUser()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, "acme-test");

        var response = await client.GetAsync("/api/v1/auth/session");
        var session = await response.Content.ReadFromJsonAsync<SessionDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(session!.User);
    }

    [Fact]
    public async Task Session_WithInvalidCookie_Returns401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, "acme-test");
        client.DefaultRequestHeaders.Add("Cookie", $"{CurrentUserMiddleware.TenantSessionCookie}=invalid-token");

        var response = await client.GetAsync("/api/v1/auth/session");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesSessionAndClearsCookie()
    {
        var (client, seed) = await CreateSeededClientAsync();
        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });
        
        var setCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var csrfCookie = setCookies.First(c => c.StartsWith("onevo_csrf="));
        var csrfToken = csrfCookie.Split(';')[0].Split('=')[1];
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        var logoutResponse = await client.PostAsync("/api/v1/auth/logout", null);
        var sessionAfterLogout = await client.GetAsync("/api/v1/auth/session");
        var session = await sessionAfterLogout.Content.ReadFromJsonAsync<SessionDto>();

        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        Assert.Null(session!.User);
    }
}
