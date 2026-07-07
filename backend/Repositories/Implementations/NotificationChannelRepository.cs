using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class NotificationChannelRepository : INotificationChannelRepository
{
    private const string ChannelTypeEmail = "email";
    private const string ProviderSendGrid = "sendgrid";

    private readonly AppDbContext _db;

    public NotificationChannelRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<NotificationChannel?> GetActiveEmailChannelAsync(Guid tenantId, CancellationToken ct)
    {
        return await _db.NotificationChannels
            .FirstOrDefaultAsync(c =>
                c.TenantId == tenantId
                && c.ChannelType == ChannelTypeEmail
                && c.Provider == ProviderSendGrid
                && c.IsActive, ct);
    }

    public async Task<NotificationChannel?> GetEmailChannelForUpdateAsync(Guid tenantId, string provider, CancellationToken ct)
    {
        return await _db.NotificationChannels
            .FirstOrDefaultAsync(c =>
                c.TenantId == tenantId
                && c.ChannelType == ChannelTypeEmail
                && c.Provider == provider, ct);
    }

    public async Task AddAsync(NotificationChannel channel)
    {
        await _db.NotificationChannels.AddAsync(channel);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
