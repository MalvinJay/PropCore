using MediatR;
using PropCore.Application.Abstractions.Messaging;
using PropCore.Domain.Common;

namespace PropCore.Infrastructure.Messaging;

public sealed record DomainEventNotification<TEvent>(TEvent Event) : INotification
    where TEvent : IDomainEvent;

public sealed class MediatRDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent);

            await publisher.Publish(notification!, cancellationToken);
        }
    }
}