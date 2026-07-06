using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Subscriptions;

public class EntitlementToggleEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public EntitlementToggleEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, SeededTenant Seed, string LoginBody)> LoginAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);
        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = seed.UserEmail, password = seed.UserPassword });
        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        var csrfCookie = loginResponse.Headers.GetValues("Set-Cookie").First(c => c.StartsWith("onevo_csrf="));
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfCookie.Split(';')[0].Split('=')[1]);

        return (client, seed, loginBody);
    }

    [Fact]
    public async Task ListEntitlements_ReturnsModulesWithFeaturesAndStates()
    {
        var (client, seed, _) = await LoginAsync();

        var response = await client.GetAsync("/api/v1/tenant/entitlements");
        var modules = await response.Content.ReadFromJsonAsync<List<Dictionary<string, System.Text.Json.JsonElement>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // The module catalog is global, so other tests' seeded modules are listed
        // too — locate this tenant's module by id, not by key.
        var widgets = modules!.First(m => m["moduleCatalogId"].GetGuid() == seed.ModuleCatalogId);
        Assert.Equal("widgets", widgets["moduleKey"].GetString());
        Assert.True(widgets["isEnabled"].GetBoolean());

        var features = widgets["features"].EnumerateArray().ToList();
        var enabledFeature = features.First(f => f.GetProperty("featureKey").GetString() == "widgets_pro");
        var disabledFeature = features.First(f => f.GetProperty("featureKey").GetString() == "widgets_export");
        Assert.True(enabledFeature.GetProperty("isEnabled").GetBoolean());
        Assert.False(disabledFeature.GetProperty("isEnabled").GetBoolean());

        // Disabled modules are still listed (so the admin can enable them),
        // and every "gadgets" module is disabled for this tenant.
        var gadgetsModules = modules.Where(m => m["moduleKey"].GetString() == "gadgets").ToList();
        Assert.NotEmpty(gadgetsModules);
        Assert.All(gadgetsModules, g => Assert.False(g["isEnabled"].GetBoolean()));
    }

    [Fact]
    public async Task DisablingModule_RemovesItAndItsFeaturesFromSession()
    {
        var (client, seed, loginBody) = await LoginAsync();

        var response = await client.PutAsJsonAsync($"/api/v1/tenant/modules/{seed.ModuleCatalogId}/entitlement", new { id = seed.ModuleCatalogId, isEnabled = false });
        var sessionResponse = await client.GetAsync("/api/v1/auth/session");
        var body = await sessionResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("\"widgets\"", body);
        Assert.DoesNotContain("widgets_pro", body);
    }

    [Fact]
    public async Task DisablingFeature_RemovesItFromSessionButKeepsModule()
    {
        var (client, seed, loginBody) = await LoginAsync();

        var response = await client.PutAsJsonAsync($"/api/v1/tenant/features/{seed.EnabledModuleFeatureId}/entitlement", new { id = seed.EnabledModuleFeatureId, isEnabled = false });
        var sessionResponse = await client.GetAsync("/api/v1/auth/session");
        var body = await sessionResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"widgets\"", body);
        Assert.DoesNotContain("widgets_pro", body);
    }
}
