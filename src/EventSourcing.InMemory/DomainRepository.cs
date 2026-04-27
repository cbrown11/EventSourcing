


namespace EventSourcing.InMemory
{
    using System.Collections.Generic;
    using System.Linq;
    using DomainDrivenDesign.Interfaces;
    using EventSourcing.Exception;
    using EventSourcing.Interfaces;
    using EventSourcing.Repository;
    using Newtonsoft.Json;

    public class DomainRepository : DomainRepositoryBase
    {
        private readonly Dictionary<string, List<string>> eventStore = new Dictionary<string, List<string>>();
        private readonly List<IDomainEvent> latestEvents = new List<IDomainEvent>();


        public DomainRepository(string category, IDomainEventPublisher? publisher = null)
            : base(category, publisher)
        {
        }

        public override async Task<IEnumerable<IDomainEvent>> SaveAsync<TAggregate>(TAggregate aggregate, bool isInitial = false, CancellationToken token = default)
        {
            var uncommitedEvents = aggregate.UncommitedEvents().ToList();
            var serializedEvents = uncommitedEvents.Select(Serialize).ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, uncommitedEvents);
            if (expectedVersion < 0)
            {
                eventStore.Add(aggregate.AggregateId, serializedEvents);
            }
            else
            {
                var existingEvents = eventStore[aggregate.AggregateId];
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new RepositoryException("Expected version " + expectedVersion + " but the version is " + currentversion);
                }

                existingEvents.AddRange(serializedEvents);
            }

            latestEvents.AddRange(uncommitedEvents);

            await this.PublishEventsToSubsribersAsync(aggregate.Name, uncommitedEvents, token);
            aggregate.ClearUncommitedEvents();
            return uncommitedEvents;
 
        }

        public override bool Exists<TResult>(string id)
        {
            return eventStore.ContainsKey(id);
        }

        public IEnumerable<IDomainEvent> GetLatestEvents()
        {
            return latestEvents;
        }

        public override async Task<TResult> GetByIdAsync<TResult>(string id, CancellationToken token = default)
        {
            return await NewMethod<TResult>(id, token);
        }

        private async Task<TResult> NewMethod<TResult>(string id, CancellationToken token) where TResult : IAggregate, new()
        {
            try
            {
                var domainEvents = await GetEventsForAggregate<TResult>(id, token);
                return BuildAggregate<TResult>(domainEvents);
            }
            catch (System.Exception ex)
            {
                throw new RepositoryException($"Unable to retrieve from eventStore for {id}", ex);
            }
        }

        public override async Task<List<IDomainEvent>> GetEventsForAggregate<TResult>(string aggregateId, CancellationToken token = default)
        {
            if (eventStore.ContainsKey(aggregateId))
            {
                var events = eventStore[aggregateId];
                return events.Select(e => JsonConvert.DeserializeObject(e, serializationSettings) as IDomainEvent).ToList();
            }

            throw new AggregateNotFoundException("Could not found aggregate of type " + typeof(TResult) + " and id " + aggregateId);
        }

        public string GetLast<TResult>()
        {
            return eventStore.Last().Key;
        }


        protected virtual async Task PublishEventsToSubsribersAsync(string aggregate, ICollection<IDomainEvent> events, CancellationToken token = default)
        {
            if (this.publisher != null)
            {
                await this.publisher.PublishAsync(aggregate, events, token);
            }
        }

        private string Serialize(IDomainEvent arg)
        {
            return JsonConvert.SerializeObject(arg, serializationSettings);
        }
    }
}
