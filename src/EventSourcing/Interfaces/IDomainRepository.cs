namespace EventSourcing.EventSourcing.Interfaces
{
    using DomainDrivenDesign.Interfaces;

    public interface IDomainRepository
    {
        Task<IEnumerable<IDomainEvent>> SaveAsync<TAggregate>(TAggregate aggregate, bool isInitial = false, CancellationToken token = default)
            where TAggregate : IAggregate;

        bool Exists<TResult>(string id)
            where TResult : IAggregate, new();

        Task<TResult> GetByIdAsync<TResult>(string id, CancellationToken token = default)
            where TResult : IAggregate, new();

        Task<List<IDomainEvent>> GetEventsForAggregate<TResult>(string aggregateId, CancellationToken token = default);
    }
}
