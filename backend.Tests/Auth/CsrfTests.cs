using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using OnevoHr.Api.Data;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Auth;

public class CsrfTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CsrfTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, string CsrfToken)> LoginAndGetCsrfAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });
        var setCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        
        var csrfCookieRaw = setCookies.First(c => c.StartsWith("onevo_csrf="));
        var csrfToken = csrfCookieRaw.Split(';')[0].Split('=')[1];

        return (client, csrfToken);
    }

    [Fact]
    public async Task UnsafeRequest_WithoutCsrfHeader_Returns403()
    {
        var (client, _) = await LoginAndGetCsrfAsync();

        var response = await client.PostAsync("/api/v1/auth/refresh", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UnsafeRequest_WithMatchingCsrfHeader_Succeeds()
    {
        var (client, csrfToken) = await LoginAndGetCsrfAsync();
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        var response = await client.PostAsync("/api/v1/auth/refresh", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SafeGetRequest_DoesNotRequireCsrf()
    {
        var (client, _) = await LoginAndGetCsrfAsync();

        var response = await client.GetAsync("/api/v1/auth/session");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
