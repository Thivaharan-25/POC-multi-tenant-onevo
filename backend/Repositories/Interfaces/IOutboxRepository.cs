using OnevoHr.Api.Models.Notifications;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IOutboxRepository
{
    Task<List<OutboxMessage>> GetPendingAsync(int limit);
    Task<List<OutboxMessage>> GetRecentAsync(int limit);
    Task AddAsync(OutboxMessage message);
    Task<List<Notification>> GetNotificationsForUserAsync(Guid tenantId, Guid userId);
    Task AddNotificationAsync(Notification notification);
    Task SaveChangesAsync();
}
