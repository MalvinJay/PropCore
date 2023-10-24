namespace PropCore.Application.Abstractions.Notifications;

public sealed record NotificationMessage(Guid UserId, string Type, string Title, string Message);

public interface INotificationService
{
    Task SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
}