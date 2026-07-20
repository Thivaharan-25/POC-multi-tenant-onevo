using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

// Hand-rolled HMAC-SHA256 (HS256) JWT, matching the codebase's from-BCL crypto
// style (see PasswordHasher/AuthService) rather than pulling in a JWT package.
public class JwtTokenService : ITokenService
{
    private static readonly TimeSpan DeviceTokenLifetime = TimeSpan.FromDays(90);
    private const string AgentTokenType = "agent";

    private static readonly JsonSerializerOptions PayloadJsonOptions = new()
    {
        PropertyNamingPolicy = null
    };

    private readonly byte[] _signingKey;

    public JwtTokenService(IConfiguration configuration)
    {
        var key = configuration["DeviceToken:SigningKey"];
        if (string.IsNullOrWhiteSpace(key))
        {
            // Deterministic dev fallback so the POC runs without extra config.
            key = "onevo-dev-device-token-signing-key-change-me-in-production";
        }

        _signingKey = Encoding.UTF8.GetBytes(key);
    }

    public string GenerateDeviceToken(Guid deviceId, Guid tenantId, out DateTimeOffset expiresAt)
    {
        var issuedAt = DateTimeOffset.UtcNow;
        expiresAt = issuedAt.Add(DeviceTokenLifetime);

        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object>
        {
            ["device_id"] = deviceId.ToString(),
            ["tenant_id"] = tenantId.ToString(),
            ["type"] = AgentTokenType,
            ["iat"] = issuedAt.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds()
        };

        var headerSegment = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header, PayloadJsonOptions));
        var payloadSegment = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload, PayloadJsonOptions));
        var signingInput = $"{headerSegment}.{payloadSegment}";
        var signatureSegment = Base64UrlEncode(ComputeSignature(signingInput));

        return $"{signingInput}.{signatureSegment}";
    }

    public DeviceTokenClaims? ValidateDeviceToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        var signingInput = $"{parts[0]}.{parts[1]}";
        var expectedSignature = Base64UrlEncode(ComputeSignature(signingInput));
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(parts[2])))
        {
            return null;
        }

        Dictionary<string, JsonElement>? payload;
        try
        {
            var payloadJson = Base64UrlDecode(parts[1]);
            payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadJson);
        }
        catch (Exception)
        {
            return null;
        }

        if (payload is null)
        {
            return null;
        }

        if (!payload.TryGetValue("exp", out var expElement) || expElement.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expElement.GetInt64());
        if (expiresAt < DateTimeOffset.UtcNow)
        {
            return null;
        }

        if (!payload.TryGetValue("device_id", out var deviceIdElement)
            || !Guid.TryParse(deviceIdElement.GetString(), out var deviceId))
        {
            return null;
        }

        if (!payload.TryGetValue("tenant_id", out var tenantIdElement)
            || !Guid.TryParse(tenantIdElement.GetString(), out var tenantId))
        {
            return null;
        }

        var type = payload.TryGetValue("type", out var typeElement) ? typeElement.GetString() : null;
        if (type != AgentTokenType)
        {
            return null;
        }

        return new DeviceTokenClaims(deviceId, tenantId, type);
    }

    private byte[] ComputeSignature(string signingInput)
    {
        using var hmac = new HMACSHA256(_signingKey);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
        }

        return Convert.FromBase64String(padded);
    }
}
