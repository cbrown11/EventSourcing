namespace EventSourcing.EventSourcing.Repository
{
    using DomainDrivenDesign.Interfaces;
    using EventSourcing.Interfaces;

    using Newtonsoft.Json;

    public abstract class DomainRepositoryBase : IDomainRepository
    {
        protected readonly IDomainEventPublisher? publisher;

        protected JsonSerializerSettings serializationSettings;

        protected readonly int noStream = -1;
        protected readonly int streamVersionStart = 0;

        private readonly string category;

        protected DomainRepositoryBase(string category, IDomainEventPublisher? publisher = null)
        {
            this.category = category;
            this.publisher = publisher;
            serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            };
        }

        protected DomainRepositoryBase(string category, int noStream, int streamVersionStart, IDomainEventPublisher? publisher = null)
            : this(category, publisher)
        {
            this.noStream = noStream;
            this.streamVersionStart = streamVersionStart;
        }

        public abstract Task<IEnumerable<IDomainEvent>> SaveAsync<TAggregate>(TAggregate aggregate, bool isInitial = false, CancellationToken token = default) where TAggregate : IAggregate;

        public abstract bool Exists<TResult>(string id)
            where TResult : IAggregate, new();

        public abstract Task<TResult> GetByIdAsync<TResult>(string id, CancellationToken token = default)
            where TResult : IAggregate, new();

        public abstract Task<List<IDomainEvent>> GetEventsForAggregate<TResult>(string aggregateId, CancellationToken token = default);

        protected virtual string AggregateToStreamName(Type type, string id)
        {
            return string.Format("{0}-{1}-{2}", this, category, type.Name, id);
        }

        protected virtual long CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            var originalVersion = aggregate.Version - events.Count + streamVersionStart;
            long expectedVersion = originalVersion == -1 ? noStream : originalVersion;
            return expectedVersion;
        }

        protected TResult BuildAggregate<TResult>(IEnumerable<IDomainEvent> events)
            where TResult : IAggregate, new()
        {
            var result = new TResult();
            foreach (var @event in events)
            {
                result.ApplyEvent(@event);
            }

            return result;
        }
    }
}
