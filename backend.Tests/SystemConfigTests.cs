using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Data.Seed;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;
using Xunit;

namespace backend.Tests;

/// <summary>
/// Tests for the platform System Config email-channel endpoints and the
/// database-backed SendGrid configuration model: the raw API key is submitted
/// once, stored encrypted in notification_channels.credentials_encrypted, and
/// never returned or committed to appsettings.
/// </summary>
public class SystemConfigTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string FakeApiKey = "SG.fake-system-config-test-key";

    private readonly CustomWebApplicationFactory _factory;

    public SystemConfigTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private async Task EnsureSeededAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!await db.PlatformUsers.AnyAsync())
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await DatabaseSeeder.SeedAsync(db, hasher);
        }
    }

    /// <summary>
    /// Logs in as the seeded Platform Super Admin and returns a client with
    /// the platform session cookie and CSRF header attached.
    /// </summary>
    private async Task<HttpClient> CreatePlatformAdminClientAsync()
    {
        await EnsureSeededAsync();

        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });

        var loginResponse = await client.PostAsJsonAsync("/admin/v1/auth/login",
            new { email = "platform.admin@onevo.test", password = "Password123!" });
        loginResponse.EnsureSuccessStatusCode();

        var csrfToken = ExtractCookieValue(loginResponse, "onevo_platform_csrf");
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);
        return client;
    }

    private static string ExtractCookieValue(HttpResponseMessage response, string cookieName)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
            throw new Exception("No Set-Cookie header on login response");
        foreach (var cookie in cookies)
        {
            if (cookie.StartsWith(cookieName + "=", StringComparison.OrdinalIgnoreCase))
                return cookie.Split(';')[0].Split('=')[1];
        }
        throw new Exception($"{cookieName} cookie not found in login response");
    }

    private async Task<Guid> GetAcmeTenantIdAsync()
    {
        await EnsureSeededAsync();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var acme = await db.Tenants.FirstAsync(t => t.Slug == "acme");
        return acme.Id;
    }

    private static object BuildUpsertRequest(Guid tenantId, string? apiKey = FakeApiKey)
    {
        return new
        {
            tenantId,
            provider = "sendgrid",
            fromEmail = "siyasiyamala932@gmail.com",
            fromName = "OneVo",
            replyToEmail = "siyasiyamala932@gmail.com",
            apiKey
        };
    }

    // ------------------------------------------------------------------
    // Auth: platform permissions required
    // ------------------------------------------------------------------

    [Fact]
    public async Task PostEmailChannel_Unauthenticated_Returns401()
    {
        await EnsureSeededAsync();
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
            BuildUpsertRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEmailChannel_Unauthenticated_Returns401()
    {
        await EnsureSeededAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/admin/v1/system-config/email-channel?tenantId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostEmailChannel_TenantSession_IsRejected()
    {
        await EnsureSeededAsync();

        // A tenant HR admin session is NOT a platform session — /admin routes
        // must reject it even though the user is authenticated in the tenant app.
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Domain", "acme.test");

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login",
            new { email = "hr.admin@acme.test", password = "Password123!" });
        loginResponse.EnsureSuccessStatusCode();
        var csrfToken = ExtractCookieValue(loginResponse, "onevo_csrf");
        client.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken);

        var response = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
            BuildUpsertRequest(await GetAcmeTenantIdAsync()));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ------------------------------------------------------------------
    // POST: stores metadata in ConfigJson, encrypts the key, never echoes it
    // ------------------------------------------------------------------

    [Fact]
    public async Task PostEmailChannel_CreatesChannel_EncryptsKey_StoresConfigJsonMetadata()
    {
        var client = await CreatePlatformAdminClientAsync();
        var tenantId = await GetAcmeTenantIdAsync();

        var response = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
            BuildUpsertRequest(tenantId));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        // Response contains safe metadata only — never the raw key
        Assert.Equal(tenantId.ToString(), root.GetProperty("tenantId").GetString());
        Assert.Equal("sendgrid", root.GetProperty("provider").GetString());
        Assert.Equal("siyasiyamala932@gmail.com", root.GetProperty("fromEmail").GetString());
        Assert.Equal("OneVo", root.GetProperty("fromName").GetString());
        Assert.True(root.GetProperty("isActive").GetBoolean());
        Assert.True(root.GetProperty("hasCredentials").GetBoolean());
        Assert.False(root.TryGetProperty("apiKey", out _));
        Assert.DoesNotContain(FakeApiKey, body);

        // DB row: metadata in ConfigJson, encrypted key in CredentialsEncrypted
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var channel = await db.NotificationChannels.FirstOrDefaultAsync(c =>
            c.TenantId == tenantId && c.ChannelType == "email" && c.Provider == "sendgrid");

        Assert.NotNull(channel);
        Assert.True(channel!.IsActive);
        Assert.Contains("siyasiyamala932@gmail.com", channel.ConfigJson);
        Assert.Contains("fromEmail", channel.ConfigJson);

        // Encrypted — never the raw key, and not even containing it
        Assert.NotEqual(FakeApiKey, channel.CredentialsEncrypted);
        Assert.DoesNotContain(FakeApiKey, channel.CredentialsEncrypted);

        // Round-trip: the processor can decrypt it back to the raw key
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();
        Assert.Equal(FakeApiKey, secretProtector.Unprotect(channel.CredentialsEncrypted));
    }

    [Fact]
    public async Task PostEmailChannel_Twice_UpdatesExistingRow_And_KeepsKey_WhenOmitted()
    {
        var client = await CreatePlatformAdminClientAsync();
        var tenantId = await GetAcmeTenantIdAsync();

        var first = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
            BuildUpsertRequest(tenantId));
        first.EnsureSuccessStatusCode();

        // Update metadata without re-submitting the key
        var second = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel", new
        {
            tenantId,
            provider = "sendgrid",
            fromEmail = "updated@onevo.test",
            fromName = "OneVo Updated",
            replyToEmail = "updated@onevo.test",
            apiKey = (string?)null
        });
        second.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var channels = await db.NotificationChannels.Where(c =>
            c.TenantId == tenantId && c.ChannelType == "email" && c.Provider == "sendgrid").ToListAsync();

        // Upsert — still exactly one row, updated metadata, credentials kept
        Assert.Single(channels);
        Assert.Contains("updated@onevo.test", channels[0].ConfigJson);
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();
        Assert.Equal(FakeApiKey, secretProtector.Unprotect(channels[0].CredentialsEncrypted));
    }

    [Fact]
    public async Task PostEmailChannel_NewChannel_WithoutApiKey_Returns400()
    {
        var client = await CreatePlatformAdminClientAsync();

        // A tenant with no channel yet — creating without a key must fail
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var freshTenant = new OnevoHr.Api.Models.Tenant.Tenant
            {
                Id = Guid.NewGuid(),
                Name = "No Key Corp",
                Slug = $"nokey-{Guid.NewGuid():N}",
                Status = "active",
                CreatedAtUtc = DateTime.UtcNow
            };
            db.Tenants.Add(freshTenant);
            await db.SaveChangesAsync();

            var response = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
                BuildUpsertRequest(freshTenant.Id, apiKey: null));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task PostEmailChannel_UnknownTenant_Returns404()
    {
        var client = await CreatePlatformAdminClientAsync();

        var response = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
            BuildUpsertRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ------------------------------------------------------------------
    // GET: safe metadata only
    // ------------------------------------------------------------------

    [Fact]
    public async Task GetEmailChannel_ReturnsMetadata_NeverTheRawKey()
    {
        var client = await CreatePlatformAdminClientAsync();
        var tenantId = await GetAcmeTenantIdAsync();

        var post = await client.PostAsJsonAsync("/admin/v1/system-config/email-channel",
            BuildUpsertRequest(tenantId));
        post.EnsureSuccessStatusCode();

        var response = await client.GetAsync($"/admin/v1/system-config/email-channel?tenantId={tenantId}");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        Assert.Equal("sendgrid", root.GetProperty("provider").GetString());
        Assert.True(root.GetProperty("hasCredentials").GetBoolean());
        Assert.False(root.TryGetProperty("apiKey", out _));
        Assert.False(root.TryGetProperty("credentialsEncrypted", out _));
        Assert.DoesNotContain(FakeApiKey, body);
    }

    [Fact]
    public async Task GetEmailChannel_NoChannelConfigured_Returns404()
    {
        var client = await CreatePlatformAdminClientAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var freshTenant = new OnevoHr.Api.Models.Tenant.Tenant
        {
            Id = Guid.NewGuid(),
            Name = "No Channel Corp",
            Slug = $"nochannel-{Guid.NewGuid():N}",
            Status = "active",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Tenants.Add(freshTenant);
        await db.SaveChangesAsync();

        var response = await client.GetAsync($"/admin/v1/system-config/email-channel?tenantId={freshTenant.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ------------------------------------------------------------------
    // appsettings must not carry SendGrid credentials
    // ------------------------------------------------------------------

    [Fact]
    public void AppSettings_Development_Contains_No_SendGrid_Credentials()
    {
        var path = FindBackendFile("appsettings.Development.json");
        var content = File.ReadAllText(path);

        Assert.DoesNotContain("SendGrid", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ApiKey", content, StringComparison.OrdinalIgnoreCase);

        // The Email section may only hold the non-secret FrontendBaseUrl
        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("Email", out var emailSection))
        {
            foreach (var property in emailSection.EnumerateObject())
            {
                Assert.Equal("FrontendBaseUrl", property.Name);
            }
        }
    }

    [Fact]
    public void AppSettings_Production_Contains_No_SendGrid_Credentials()
    {
        var path = FindBackendFile("appsettings.json");
        var content = File.ReadAllText(path);

        Assert.DoesNotContain("SendGrid", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ApiKey", content, StringComparison.OrdinalIgnoreCase);
    }

    private static string FindBackendFile(string fileName)
    {
        // Walk up from the test bin directory to the repo checkout, then into backend/.
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "backend", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
            directory = directory.Parent;
        }
        throw new FileNotFoundException($"Could not locate backend/{fileName} above {AppContext.BaseDirectory}");
    }
}
