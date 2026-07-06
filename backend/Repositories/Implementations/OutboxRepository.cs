using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Notifications;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _db;
    private static readonly ConcurrentBag<OutboxMessage> _messages = new();

    public OutboxRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<OutboxMessage>> GetPendingAsync(int limit)
    {
        var pending = _messages
            .Where(m => m.Status == "pending")
            .OrderBy(m => m.CreatedAtUtc)
            .Take(limit)
            .ToList();
        return Task.FromResult(pending);
    }

    public Task AddAsync(OutboxMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
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

    public static List<OutboxMessage> GetRecentMessages()
    {
        return _messages.OrderByDescending(m => m.CreatedAtUtc).Take(50).ToList();
    }
}
