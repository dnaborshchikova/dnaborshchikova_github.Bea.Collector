using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class BillEventFileMQSender : IEventSender
    {
        public void Send(BillEvent billEvent)
        {
            Thread.Sleep(1000);

            var billData = billEvent switch
            {
                PaidBillEvent paid => JsonSerializer.Serialize(paid),
                CancelledBillEvent cancelled => JsonSerializer.Serialize(cancelled)
            };

            var sendEvent = new SendEvent(billEvent.Id, billEvent.OperationDateTime,
                billEvent.UserId, billEvent.EventType, billData);
        }
    }
}
