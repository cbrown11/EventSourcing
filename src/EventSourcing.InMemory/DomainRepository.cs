using System.Collections.Generic;
using System.Linq;
using DomainDrivenDesign.Interfaces;
using EventSourcing.EventSourcing.Exception;
using EventSourcing.EventSourcing.Interfaces;
using EventSourcing.EventSourcing.Repository;

using Newtonsoft.Json;


namespace EventSourcing.InMemory
{

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
            aggregate.ClearUncommitedEvents();
            foreach (IDomainEvent @event in uncommitedEvents)
            {
                await PublishEventAsync(@event, token);
            }

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
            try
            {
                var domainEvents = await GetEventsForAggregate<TResult>(id, token);
                return BuildAggregate<TResult>(domainEvents);
            }
            catch (Exception ex)
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

        protected async Task PublishEventAsync(IDomainEvent @event, CancellationToken token = default)
        {
            if (publisher != null)
            {
                await publisher.PublishAsync(@event, token);
            }
        }

        private string Serialize(IDomainEvent arg)
        {
            return JsonConvert.SerializeObject(arg, serializationSettings);
        }
    }
}
