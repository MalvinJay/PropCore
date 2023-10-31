using Microsoft.Extensions.Logging;
using PropCore.Application.Abstractions.Notifications;

namespace PropCore.Infrastructure.Notifications;

public sealed class InMemoryNotificationService(ILogger<InMemoryNotificationService> logger) : INotificationService
{
    public Task SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Sending notification of type {Type} to user {UserId}: {Title}",
            notification.Type,
            notification.UserId,
            notification.Title);

        return Task.CompletedTask;
    }
}