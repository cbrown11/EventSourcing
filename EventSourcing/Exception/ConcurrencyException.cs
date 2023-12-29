namespace EventSourcing.EventSourcing.Exception
{
    public class ConcurrencyException : System.Exception
    {
        public ConcurrencyException()
            : base()
        {
        }

        public ConcurrencyException(string? message)
            : base(message)
        {
        }

        public ConcurrencyException(string? message, System.Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
