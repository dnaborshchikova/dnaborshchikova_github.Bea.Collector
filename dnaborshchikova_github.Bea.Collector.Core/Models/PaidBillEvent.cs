namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class PaidBillEvent : BillEvent
    {
        public Guid? BuyerId { get; set; }
    }
}
