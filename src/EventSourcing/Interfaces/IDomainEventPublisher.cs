
using DomainDrivenDesign.Interfaces;

namespace EventSourcing.Interfaces
{

    public interface IDomainEventPublisher
    {
        Task PublishAsync<T>(string domainName, IEnumerable<T> events, CancellationToken token = default) where T : IDomainEvent;
    }
}
