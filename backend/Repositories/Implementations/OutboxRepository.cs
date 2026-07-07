using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Notifications;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _db;

    public OutboxRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<OutboxMessage>> GetPendingAsync(int limit)
    {
        return await _db.OutboxMessages
            .Where(m => m.Status == "pending")
            .OrderBy(m => m.CreatedAtUtc)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<OutboxMessage>> GetRecentAsync(int limit)
    {
        return await _db.OutboxMessages.AsNoTracking()
            .OrderByDescending(m => m.CreatedAtUtc)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(OutboxMessage message)
    {
        // Intentionally no SaveChanges here: the caller's unit of work commits the
        // outbox row in the same transaction as the business rows it belongs to.
        await _db.OutboxMessages.AddAsync(message);
    }

    public async Task<List<Notification>> GetNotificationsForUserAsync(Guid tenantId, Guid userId)
    {
        return await _db.Notifications.AsNoTracking()
            .Where(n => n.TenantId == tenantId && (n.UserId == null || n.UserId == userId))
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await _db.Notifications.AddAsync(notification);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
