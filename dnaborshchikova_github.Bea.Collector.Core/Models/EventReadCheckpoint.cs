namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class EventReadCheckpoint
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public Guid LastEventId { get; set; }
        public DateTime LastReadDate { get; set; }

        public EventReadCheckpoint(string fileName, Guid lastEventId, DateTime lastReadDate)
        {
            var utcTime = DateTime.SpecifyKind(lastReadDate, DateTimeKind.Utc);

            this.FileName = fileName;
            this.LastEventId = lastEventId;
            this.LastReadDate = utcTime;
        }
    }
}
