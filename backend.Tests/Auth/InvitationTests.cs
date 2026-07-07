using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using OnevoHr.Api.Data;
using OnevoHr.Api.DTOs.Auth;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;

namespace OnevoHr.Api.Tests.Auth;

public class InvitationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InvitationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, SeededTenant Seed, InvitationToken Invite, string RawToken)> SetupTestInviteAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seed = await TestDataSeeder.SeedMinimalTenantAsync(db, hasher);

        client.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        // Create an invited user
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = seed.TenantId,
            Email = "invited@acme.com",
            DisplayName = "Invited User",
            PasswordHash = string.Empty,
            IsActive = false,
            PasswordSetupRequired = true,
            PasswordSetupExpiresAt = DateTimeOffset.UtcNow.AddHours(72),
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(newUser);

        var rawToken = $"test_raw_token_{Guid.NewGuid()}";
        var tokenHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken))).ToLowerInvariant();

        var inviteToken = new InvitationToken
        {
            Id = Guid.NewGuid(),
            TenantId = seed.TenantId,
            UserId = newUser.Id,
            InvitedEmail = newUser.Email,
            InvitedFullName = newUser.DisplayName,
            TokenHash = tokenHash,
            Status = "pending",
            CompletionMethodsJson = "[\"password\"]",
            AllowedEmailDomainsJson = "[]",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(72),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUserId = seed.UserId
        };
        db.InvitationTokens.Add(inviteToken);
        await db.SaveChangesAsync();

        return (client, seed, inviteToken, rawToken);
    }

    [Fact]
    public async Task ValidateInvite_WithValidToken_ReturnsSafeInviteDetails()
    {
        var (client, _, _, rawToken) = await SetupTestInviteAsync();

        var response = await client.GetAsync($"/api/v1/auth/invitations/validate?token={rawToken}");
        var content = await response.Content.ReadFromJsonAsync<SafeInvitationDto>();

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected success, got {response.StatusCode}. Body: {body}");
        Assert.NotNull(content);
        Assert.Equal("invited@acme.com", content!.InvitedEmail);
        Assert.Contains("password", content.AllowedCompletionMethods);
    }

    [Fact]
    public async Task ValidateInvite_WithInvalidToken_Returns404Or400()
    {
        var (client, _, _, _) = await SetupTestInviteAsync();

        var response = await client.GetAsync($"/api/v1/auth/invitations/validate?token=invalid_token");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidateInvite_WithExpiredToken_ReturnsInvalid()
    {
        var (client, _, inviteToken, rawToken) = await SetupTestInviteAsync();
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var invite = await db.InvitationTokens.FindAsync(inviteToken.Id);
        invite!.ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1);
        await db.SaveChangesAsync();

        var response = await client.GetAsync($"/api/v1/auth/invitations/validate?token={rawToken}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvite_WithValidPassword_ActivatesUserAndMarksInviteUsed()
    {
        var (client, _, inviteToken, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var user = await db.Users.FindAsync(inviteToken.UserId);
        Assert.True(user!.IsActive);
        Assert.False(user.PasswordSetupRequired);
        Assert.Null(user.PasswordSetupExpiresAt);

        var updatedInvite = await db.InvitationTokens.FindAsync(inviteToken.Id);
        Assert.Equal("completed", updatedInvite!.Status);
        Assert.NotNull(updatedInvite.UsedAt);
    }

    [Fact]
    public async Task AcceptInvite_StoresPasswordHash_NotPlainPassword()
    {
        var (client, _, inviteToken, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.FindAsync(inviteToken.UserId);
        
        Assert.False(string.IsNullOrWhiteSpace(user!.PasswordHash));
        Assert.NotEqual("StrongPass1!", user.PasswordHash);
    }

    [Fact]
    public async Task AcceptInvite_DoesNotReturnRawTokenOrTokenHash()
    {
        var (client, _, _, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);
        var body = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("tokenHash", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(rawToken, body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptInvite_WithAlreadyUsedToken_Fails()
    {
        var (client, seed, _, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);
        
        var client2 = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        client2.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        var response2 = await client2.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }

    [Fact]
    public async Task AcceptInvite_WithExpiredToken_Fails()
    {
        var (client, _, inviteToken, rawToken) = await SetupTestInviteAsync();
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var invite = await db.InvitationTokens.FindAsync(inviteToken.Id);
        invite!.ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1);
        await db.SaveChangesAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvite_WithWeakPassword_Fails()
    {
        var (client, _, _, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "weak", "weak");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvite_WithPasswordMismatch_Fails()
    {
        var (client, _, _, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass2!");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AcceptInvite_WhenCompletionMethodsDoesNotContainPassword_Fails()
    {
        var (client, _, inviteToken, rawToken) = await SetupTestInviteAsync();
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var invite = await db.InvitationTokens.FindAsync(inviteToken.Id);
        invite!.CompletionMethodsJson = "[\"google\"]";
        await db.SaveChangesAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_BeforeInviteAccepted_Fails()
    {
        var (client, seed, inviteToken, _) = await SetupTestInviteAsync();

        // The user exists but IsActive is false and PasswordHash is empty
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = inviteToken.InvitedEmail, password = "StrongPass1!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_AfterInviteAccepted_Succeeds_AndSetsHttpOnlySessionCookie_AndCsrfCookie()
    {
        var (client, seed, inviteToken, rawToken) = await SetupTestInviteAsync();

        // Accept invite
        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        // Ensure we clear previous cookies for a fresh login test
        var loginClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, BaseAddress = new Uri("https://localhost") });
        loginClient.DefaultRequestHeaders.Add(TenantResolutionMiddleware.TenantDomainHeader, seed.TenantSlug);

        // Login
        var response = await loginClient.PostAsJsonAsync("/api/v1/auth/login", new { email = inviteToken.InvitedEmail, password = "StrongPass1!" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var setCookies = response.Headers.TryGetValues("Set-Cookie", out var cookies) ? cookies.ToList() : new List<string>();
        Assert.Contains(setCookies, c => c.StartsWith($"{CurrentUserMiddleware.TenantSessionCookie}=") && c.Contains("httponly", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(setCookies, c => c.StartsWith("onevo_csrf="));
    }

    [Fact]
    public async Task AcceptInvite_DoesNotCreateDuplicateSessionUnlessEndpointIsDesignedToLoginImmediately()
    {
        // The current implementation is designed to login immediately
        var (client, _, _, rawToken) = await SetupTestInviteAsync();

        var request = new AcceptInviteRequestDto(rawToken, "StrongPass1!", "StrongPass1!");
        var response = await client.PostAsJsonAsync("/api/v1/auth/invitations/accept", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var setCookies = response.Headers.TryGetValues("Set-Cookie", out var cookies) ? cookies.ToList() : new List<string>();
        Assert.Contains(setCookies, c => c.StartsWith($"{CurrentUserMiddleware.TenantSessionCookie}=") && c.Contains("httponly", StringComparison.OrdinalIgnoreCase));
    }
}
