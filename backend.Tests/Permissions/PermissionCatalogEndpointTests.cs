using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Permissions;

public class PermissionCatalogEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PermissionCatalogEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, SeededTenant Seed)> LoginAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });
        var csrfCookie = loginResponse.Headers.GetValues("Set-Cookie").First(c => c.StartsWith("onevo_csrf="));
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfCookie.Split(';')[0].Split('=')[1]);

        return (client, seed);
    }

    [Fact]
    public async Task Catalog_UsesFrontendContractPropertyNames()
    {
        var (client, _) = await LoginAsync();

        var response = await client.GetAsync("/api/v1/permissions/catalog");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(document.RootElement.GetArrayLength() > 0, "Catalog should not be empty for the seeded tenant.");

        foreach (var entry in document.RootElement.EnumerateArray())
        {
            Assert.True(entry.TryGetProperty("id", out _));
            Assert.True(entry.TryGetProperty("code", out var code));
            Assert.True(entry.TryGetProperty("description", out _));
            Assert.True(entry.TryGetProperty("module", out _));
            Assert.True(entry.TryGetProperty("featureKey", out _));
            Assert.False(string.IsNullOrWhiteSpace(code.GetString()));

            // Old backend-internal names must be gone from the wire contract.
            Assert.False(entry.TryGetProperty("permissionKey", out _));
            Assert.False(entry.TryGetProperty("displayName", out _));
            Assert.False(entry.TryGetProperty("category", out _));
        }
    }

    [Fact]
    public async Task Catalog_ExcludesPermissionsOfDisabledModulesAndFeatures()
    {
        var (client, _) = await LoginAsync();

        var response = await client.GetAsync("/api/v1/permissions/catalog");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("widgets:read", body);
        Assert.Contains("widgets:manage", body);
        Assert.DoesNotContain("widgets:export", body); // gated by a disabled feature
        Assert.DoesNotContain("gadgets:read", body);   // owned by a disabled module
    }
}
