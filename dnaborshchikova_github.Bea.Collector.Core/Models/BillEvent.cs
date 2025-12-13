using System.Text.Json.Serialization;

namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class BillEvent
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public DateTime OperationDateTime { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public string EventType { get; set; } // "bill_payed" или "bill_canceled"

        public decimal Amount { get; set; }

        public string Number { get; set; }

        public BillEvent()
        {

        }

        public BillEvent(Guid id, DateTime operationDateTime, Guid userId, string eventType
            , decimal amount, string number)
        {
            Id = id;
            OperationDateTime = operationDateTime;
            UserId = userId;
            EventType = eventType;
            Amount = amount;
            Number = number;
        }
    }
}
