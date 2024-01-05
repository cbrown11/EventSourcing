namespace EventSourcing.EventSourcing.Exception
{
    public class AggregateNotFoundException : System.Exception
    {
        public AggregateNotFoundException(string message)
            : base(message)
        {
        }
    }
}
