using System;
using System.Threading;
using System.Threading.Tasks;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface INotificationChannelRepository
{
    /// <summary>
    /// Returns the active SendGrid email channel for the tenant, or null when
    /// no active channel is configured (the local_dev fallback applies).
    /// </summary>
    Task<NotificationChannel?> GetActiveEmailChannelAsync(Guid tenantId, CancellationToken ct);

    /// <summary>
    /// Returns the tenant's email channel for the given provider regardless of
    /// IsActive, so System Config upserts update the existing row.
    /// </summary>
    Task<NotificationChannel?> GetEmailChannelForUpdateAsync(Guid tenantId, string provider, CancellationToken ct);

    Task AddAsync(NotificationChannel channel);

    Task SaveChangesAsync(CancellationToken ct = default);
}
