
namespace EventSourcing.EventSourcing.Interfaces
{
    using DomainDrivenDesign.DDD.Interfaces;

    public interface IDomainEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken token = default) where T : IDomainEvent;
    }
}
