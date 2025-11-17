namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class BillEvent
    {
        public Guid Id { get; set; }
        public DateTime OperationDateTime { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; } // "bill_payed" или "bill_canceled"
        public decimal Amount { get; set; }
        public string Number { get; set; }        
    }
}
