using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Permissions;

public class PermissionOverrideEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PermissionOverrideEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, SeededTenant Seed, string Csrf)> LoginAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });
        var setCookies = loginResponse.Headers.GetValues("Set-Cookie").ToList();
        var csrfCookie = setCookies.First(c => c.StartsWith("onevo_csrf="));
        var csrfToken = csrfCookie.Split(';')[0].Split('=')[1];
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        return (client, seed, csrfToken);
    }

    [Fact]
    public async Task GrantOverride_ThenListOverrides_ReturnsIt()
    {
        var (client, seed, _) = await LoginAsync();

        var createResponse = await client.PostAsJsonAsync("/api/v1/permissions/overrides", new
        {
            userId = seed.UserId,
            permissionId = seed.GatedEnabledPermissionId,
            grantType = "grant",
            reason = "temporary access for testing",
            validFrom = (DateTimeOffset?)null,
            expiresAt = (DateTimeOffset?)null
        });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var listResponse = await client.GetAsync($"/api/v1/permissions/overrides/{seed.UserId}");
        var overrides = await listResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Single(overrides!);
    }

    [Fact]
    public async Task CreateOverride_ForDisabledFeaturePermission_Returns400()
    {
        var (client, seed, _) = await LoginAsync();

        var createResponse = await client.PostAsJsonAsync("/api/v1/permissions/overrides", new
        {
            userId = seed.UserId,
            permissionId = seed.GatedDisabledPermissionId,
            grantType = "grant",
            reason = "should be rejected",
            validFrom = (DateTimeOffset?)null,
            expiresAt = (DateTimeOffset?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
    }

    [Fact]
    public async Task CreateOverride_ForDisabledModulePermission_Returns400()
    {
        var (client, seed, _) = await LoginAsync();

        var createResponse = await client.PostAsJsonAsync("/api/v1/permissions/overrides", new
        {
            userId = seed.UserId,
            permissionId = seed.DisabledModulePermissionId,
            grantType = "grant",
            reason = "should be rejected",
            validFrom = (DateTimeOffset?)null,
            expiresAt = (DateTimeOffset?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
    }

    [Fact]
    public async Task RevokeOverride_RemovesIt()
    {
        var (client, seed, _) = await LoginAsync();
        var createResponse = await client.PostAsJsonAsync("/api/v1/permissions/overrides", new
        {
            userId = seed.UserId,
            permissionId = seed.GatedEnabledPermissionId,
            grantType = "grant",
            reason = "temporary access for testing",
            validFrom = (DateTimeOffset?)null,
            expiresAt = (DateTimeOffset?)null
        });
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var overrideId = created!["id"].ToString();

        var deleteResponse = await client.DeleteAsync($"/api/v1/permissions/overrides/{overrideId}");
        var listResponse = await client.GetAsync($"/api/v1/permissions/overrides/{seed.UserId}");
        var overrides = await listResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Empty(overrides!);
    }
}
