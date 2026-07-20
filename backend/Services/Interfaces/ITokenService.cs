namespace OnevoHr.Api.Services.Interfaces;

// Issues and validates the internal device credential (a signed JWT) the tray
// agent uses to authenticate /api/v1/agent/* requests. Claims are exactly
// device_id, tenant_id, type="agent" - it carries no HR permissions and is not
// a user token. See docs/superpowers/specs/2026-07-11-agent-service-split-jwt-design.md.
public interface ITokenService
{
    string GenerateDeviceToken(Guid deviceId, Guid tenantId, out DateTimeOffset expiresAt);

    DeviceTokenClaims? ValidateDeviceToken(string token);
}

public sealed record DeviceTokenClaims(Guid DeviceId, Guid TenantId, string Type);
