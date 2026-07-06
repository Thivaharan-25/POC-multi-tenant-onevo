using OnevoHr.Api.DTOs.Notifications;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IOutboxRepository _outbox;
    private readonly ICurrentUserService _currentUser;

    public NotificationService(IOutboxRepository outbox, ICurrentUserService currentUser)
    {
        _outbox = outbox;
        _currentUser = currentUser;
    }

    public async Task<List<NotificationDto>> GetMyNotificationsAsync()
    {
        if (!_currentUser.IsAuthenticated)
        {
            return new List<NotificationDto>();
        }

        var notifications = await _outbox.GetNotificationsForUserAsync(
            _currentUser.TenantId!.Value,
            _currentUser.UserId!.Value);

        return notifications
            .Select(n => new NotificationDto(n.Id, n.Title, n.Body, n.IsRead, n.CreatedAtUtc))
            .ToList();
    }
}
