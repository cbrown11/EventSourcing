
using DomainDrivenDesign.Shared.Interfaces;

namespace EventSourcing.EventSourcing
{
    public class EventStoreEvent<T>
        where T : IEvent
    {
        public EventStoreEvent(int version, T @event)
        {
            Event = @event;
            Version = version;
        }

        public T Event { get; set; }

        public int Version { get; set; }
    }
}
