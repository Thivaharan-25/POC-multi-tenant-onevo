using OnevoHr.Api.Models.Notifications;

namespace OnevoHr.Api.Services.Notifications;

public interface IOutboxService
{
    Task EnqueueAsync(Guid? tenantId, string type, string payloadJson);
    Task<List<OutboxMessage>> GetPendingAsync(int limit);
    Task MarkPublishedAsync(OutboxMessage message);
    Task MarkFailedAsync(OutboxMessage message);
}
