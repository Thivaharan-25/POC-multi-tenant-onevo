using OnevoHr.Api.Models.Notifications;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Services.Notifications;

public class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _outbox;

    public OutboxService(IOutboxRepository outbox)
    {
        _outbox = outbox;
    }

    public async Task EnqueueAsync(Guid? tenantId, string type, string payloadJson)
    {
        await _outbox.AddAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = type,
            PayloadJson = payloadJson,
            Status = "pending",
            RetryCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        });

        // Standalone enqueues (e.g. login_succeeded) are not part of a wider unit
        // of work, so the message is committed here. Flows that must stay atomic
        // with other rows add to IOutboxRepository directly and save once.
        await _outbox.SaveChangesAsync();
    }

    public async Task<List<OutboxMessage>> GetPendingAsync(int limit)
    {
        return await _outbox.GetPendingAsync(limit);
    }

    public async Task MarkPublishedAsync(OutboxMessage message)
    {
        message.Status = "published";
        message.PublishedAtUtc = DateTime.UtcNow;
        await _outbox.SaveChangesAsync();
    }

    public async Task MarkFailedAsync(OutboxMessage message)
    {
        message.RetryCount++;
        message.Status = message.RetryCount >= 5 ? "failed" : "pending";
        await _outbox.SaveChangesAsync();
    }
}
