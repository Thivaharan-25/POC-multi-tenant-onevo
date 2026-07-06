using OnevoHr.Api.DTOs.Notifications;

namespace OnevoHr.Api.Services.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetMyNotificationsAsync();
}
