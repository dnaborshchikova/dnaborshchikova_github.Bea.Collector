namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class ProcessingException : Exception
    {
        public ProcessingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
