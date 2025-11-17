namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }
    }
}
