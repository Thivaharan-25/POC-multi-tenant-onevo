using System;
using System.Threading;
using System.Threading.Tasks;
using OnevoHr.Api.DTOs.Admin;

namespace OnevoHr.Api.Services.Interfaces;

/// <summary>
/// Platform System Config operations. Currently only the per-tenant email
/// provider channel (notification_channels row) is managed here.
/// </summary>
public interface ISystemConfigService
{
    /// <summary>
    /// Create or update the tenant's active email notification channel.
    /// Returns null when the tenant does not exist. Throws
    /// ArgumentException when a new channel is submitted without an API key.
    /// </summary>
    Task<EmailChannelResponse?> UpsertEmailChannelAsync(UpsertEmailChannelRequest request, Guid configuredById, CancellationToken ct);

    /// <summary>Safe metadata for the tenant's email channel, or null when none exists.</summary>
    Task<EmailChannelResponse?> GetEmailChannelAsync(Guid tenantId, CancellationToken ct);
}
