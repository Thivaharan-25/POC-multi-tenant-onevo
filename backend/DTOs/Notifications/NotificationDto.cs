namespace OnevoHr.Api.DTOs.Notifications;

public sealed record NotificationDto(Guid Id, string Title, string Body, bool IsRead, DateTime CreatedAtUtc);
