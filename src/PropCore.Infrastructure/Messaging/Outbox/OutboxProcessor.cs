using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropCore.Infrastructure.Persistence;

namespace PropCore.Infrastructure.Messaging.Outbox;

public sealed class OutboxProcessor(
    IDbContextFactory<PropCoreDbContext> contextFactory,
    IBus bus,
    ILogger<OutboxProcessor> logger)
{
    private const int BatchSize = 50;

    public async Task ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var pendingMessages = await context.OutboxMessages
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.OccurredOn)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                var eventType = Type.GetType(message.Type);

                object? integrationEvent = null;

                if (eventType is not null)
                {
                    integrationEvent = System.Text.Json.JsonSerializer.Deserialize(
                        message.Payload,
                        eventType);
                }

                if (integrationEvent is not null)
                {
                    await bus.Publish(integrationEvent, cancellationToken);
                }

                message.ProcessedOn = DateTime.UtcNow;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Failed to publish outbox message {MessageId} of type {MessageType}",
                    message.Id,
                    message.Type);

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = exception.Message;
            }
        }

        if (pendingMessages.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}