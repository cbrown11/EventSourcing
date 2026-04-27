namespace EventSourcing.Decorators
{
    using DomainDrivenDesign.Interfaces;
    using EventSourcing.Interfaces;


    public class PublisherPublishEventsOnlyDecorator : IDomainEventPublisher
    {
        private readonly IDomainEventPublisher domainEventPublisher;

        public PublisherPublishEventsOnlyDecorator(IDomainEventPublisher domainEventPublisher)
        {
            this.domainEventPublisher = domainEventPublisher;
        }

        public Task PublishAsync<T>(string domainName, IEnumerable<T> events, CancellationToken token = default) where T : IDomainEvent
        {
            var filterEvents = events.Where(x => x is IPublishEvent);
            if (!filterEvents.Any())
            {
                return Task.CompletedTask;
            }

            return domainEventPublisher.PublishAsync(domainName, filterEvents, token);
        }

    }
}
