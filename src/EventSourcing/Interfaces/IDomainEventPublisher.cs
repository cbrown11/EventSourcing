
using DomainDrivenDesign.Interfaces;

namespace EventSourcing.EventSourcing.Interfaces
{


    public interface IDomainEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken token = default) where T : IDomainEvent;
    }
}
