using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Permissions;

public class RoleManagementEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RoleManagementEndpointTests(CustomWebApplicationFactory factory)
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
    public async Task CreateRole_ThenSetPermissions_ReflectsInRoleList()
    {
        var (client, seed) = await LoginAsync();

        var createResponse = await client.PostAsJsonAsync("/api/v1/roles", new { name = "Reviewer", description = "Can review widgets" });
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var roleId = created!["id"].ToString();

        var setPermsResponse = await client.PutAsJsonAsync($"/api/v1/roles/{roleId}/permissions", new { permissionIds = new[] { seed.UngatedPermissionId } });

        var listResponse = await client.GetAsync("/api/v1/roles");
        var roles = await listResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        var reviewerRole = roles!.First(r => r["name"].ToString() == "Reviewer");

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, setPermsResponse.StatusCode);
        Assert.Contains("widgets:read", ((System.Text.Json.JsonElement)reviewerRole["permissionCodes"]).EnumerateArray().Select(e => e.GetString()));
        Assert.Contains(seed.UngatedPermissionId.ToString(), ((System.Text.Json.JsonElement)reviewerRole["permissionIds"]).EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public async Task RoleList_ReturnsPermissionIdsAndCodesInSeparateFields()
    {
        var (client, seed) = await LoginAsync();

        var listResponse = await client.GetAsync("/api/v1/roles");
        var roles = await listResponse.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        var testerRole = roles!.First(r => r["name"].ToString() == "Tester");

        var permissionIds = ((System.Text.Json.JsonElement)testerRole["permissionIds"]).EnumerateArray().Select(e => e.GetString()).ToList();
        var permissionCodes = ((System.Text.Json.JsonElement)testerRole["permissionCodes"]).EnumerateArray().Select(e => e.GetString()).ToList();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.False(testerRole.ContainsKey("permissions"));
        Assert.Contains(seed.UngatedPermissionId.ToString(), permissionIds);
        Assert.Contains("widgets:read", permissionCodes);
        // Every entry in permissionIds must be a GUID, never a code string.
        Assert.All(permissionIds, id => Assert.True(Guid.TryParse(id, out _)));
    }

    [Fact]
    public async Task SetRolePermissions_WithDisabledFeaturePermission_Returns400()
    {
        var (client, seed) = await LoginAsync();
        var createResponse = await client.PostAsJsonAsync("/api/v1/roles", new { name = "Reviewer", description = "Can review widgets" });
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var roleId = created!["id"].ToString();

        var setPermsResponse = await client.PutAsJsonAsync($"/api/v1/roles/{roleId}/permissions", new { permissionIds = new[] { seed.GatedDisabledPermissionId } });

        Assert.Equal(HttpStatusCode.BadRequest, setPermsResponse.StatusCode);
    }

    [Fact]
    public async Task SetRolePermissions_WithDisabledModulePermission_Returns400()
    {
        var (client, seed) = await LoginAsync();
        var createResponse = await client.PostAsJsonAsync("/api/v1/roles", new { name = "Reviewer", description = "Can review widgets" });
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var roleId = created!["id"].ToString();

        var setPermsResponse = await client.PutAsJsonAsync($"/api/v1/roles/{roleId}/permissions", new { permissionIds = new[] { seed.DisabledModulePermissionId } });

        Assert.Equal(HttpStatusCode.BadRequest, setPermsResponse.StatusCode);
    }

    [Fact]
    public async Task AssignUserRole_GrantsRolePermissions()
    {
        var (client, seed) = await LoginAsync();
        var createResponse = await client.PostAsJsonAsync("/api/v1/roles", new { name = "Reviewer", description = "Can review widgets" });
        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var roleId = Guid.Parse(created!["id"].ToString()!);
        await client.PutAsJsonAsync($"/api/v1/roles/{roleId}/permissions", new { permissionIds = new[] { seed.GatedEnabledPermissionId } });

        var assignResponse = await client.PostAsJsonAsync("/api/v1/roles/assignments", new { userId = seed.UserId, roleId });

        var sessionResponse = await client.GetAsync("/api/v1/auth/session");
        var body = await sessionResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);
        Assert.Contains("widgets:manage", body);
    }
}
