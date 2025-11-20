namespace dnaborshchikova_github.Bea.Collector.Generator.Models
{
    public class BillEventFileRecord
    {
        public Guid Id { get; set; }
        public DateTime OperationDateTime { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; }
        public decimal Amount { get; set; }
        public string BillNumber { get; set; }

        public Guid? BuyerId { get; set; }
        public decimal? CancelAmount { get; set; }
    }
}
