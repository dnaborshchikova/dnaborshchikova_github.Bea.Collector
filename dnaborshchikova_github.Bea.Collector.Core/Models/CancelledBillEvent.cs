namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class CancelledBillEvent : BillEvent
    {
        public decimal? CancelAmount { get; set; }
    }
}
