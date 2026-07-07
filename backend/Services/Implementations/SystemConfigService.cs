using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Manages the per-tenant email provider channel stored in
/// notification_channels. Non-secret metadata goes into ConfigJson; the raw
/// API key is encrypted with ISecretProtector into CredentialsEncrypted and
/// is never returned, never logged.
/// </summary>
public class SystemConfigService : ISystemConfigService
{
    private const string ChannelTypeEmail = "email";

    private readonly INotificationChannelRepository _notificationChannelRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ISecretProtector _secretProtector;

    public SystemConfigService(
        INotificationChannelRepository notificationChannelRepository,
        ITenantRepository tenantRepository,
        ISecretProtector secretProtector)
    {
        _notificationChannelRepository = notificationChannelRepository;
        _tenantRepository = tenantRepository;
        _secretProtector = secretProtector;
    }

    public async Task<EmailChannelResponse?> UpsertEmailChannelAsync(UpsertEmailChannelRequest request, Guid configuredById, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId);
        if (tenant is null)
        {
            return null;
        }

        var provider = request.Provider.Trim().ToLowerInvariant();
        var channel = await _notificationChannelRepository.GetEmailChannelForUpdateAsync(request.TenantId, provider, ct);

        var isNewChannel = channel is null;
        if (isNewChannel && string.IsNullOrWhiteSpace(request.ApiKey))
        {
            throw new ArgumentException("An API key is required when configuring a new email channel.");
        }

        if (channel is null)
        {
            channel = new NotificationChannel
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                ChannelType = ChannelTypeEmail,
                Provider = provider,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _notificationChannelRepository.AddAsync(channel);
        }

        channel.ConfigJson = JsonSerializer.Serialize(new
        {
            fromEmail = request.FromEmail,
            fromName = request.FromName,
            replyToEmail = request.ReplyToEmail
        });

        // The raw key is encrypted immediately and only the protected value is
        // persisted. An omitted key on update keeps the stored credentials.
        if (!string.IsNullOrWhiteSpace(request.ApiKey))
        {
            channel.CredentialsEncrypted = _secretProtector.Protect(request.ApiKey);
        }

        channel.IsActive = true;
        channel.ConfiguredById = configuredById;
        channel.UpdatedAt = DateTimeOffset.UtcNow;

        await _notificationChannelRepository.SaveChangesAsync(ct);

        return BuildResponse(channel);
    }

    public async Task<EmailChannelResponse?> GetEmailChannelAsync(Guid tenantId, CancellationToken ct)
    {
        var channel = await _notificationChannelRepository.GetActiveEmailChannelAsync(tenantId, ct);
        if (channel is null)
        {
            return null;
        }

        return BuildResponse(channel);
    }

    private static EmailChannelResponse BuildResponse(NotificationChannel channel)
    {
        var fromEmail = string.Empty;
        var fromName = string.Empty;
        string? replyToEmail = null;

        if (!string.IsNullOrWhiteSpace(channel.ConfigJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(channel.ConfigJson);
                var root = doc.RootElement;
                fromEmail = ReadStringProperty(root, "fromEmail") ?? string.Empty;
                fromName = ReadStringProperty(root, "fromName") ?? string.Empty;
                replyToEmail = ReadStringProperty(root, "replyToEmail");
            }
            catch (JsonException)
            {
                // Corrupt metadata still yields a safe (empty) response.
            }
        }

        return new EmailChannelResponse(
            channel.Id,
            channel.TenantId,
            channel.Provider,
            fromEmail,
            fromName,
            replyToEmail,
            channel.IsActive,
            !string.IsNullOrEmpty(channel.CredentialsEncrypted));
    }

    private static string? ReadStringProperty(JsonElement root, string propertyName)
    {
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
        {
            return property.GetString();
        }
        return null;
    }
}
