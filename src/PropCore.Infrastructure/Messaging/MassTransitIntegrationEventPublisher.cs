using MassTransit;
using PropCore.Application.Abstractions.Messaging;

namespace PropCore.Infrastructure.Messaging;

public sealed class MassTransitIntegrationEventPublisher(IBus bus) : IIntegrationEventPublisher
{
    public Task PublishAsync(object integrationEvent, CancellationToken cancellationToken = default)
    {
        return bus.Publish(integrationEvent, cancellationToken);
    }
}