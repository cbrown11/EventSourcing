namespace EventSourcing.EventSourcing.Exception
{
    [Serializable]
    public class RepositoryException : System.Exception
    {
        public RepositoryException()
        {
        }

        public RepositoryException(string message)
            : base(message)
        {
        }

        public RepositoryException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        protected RepositoryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
