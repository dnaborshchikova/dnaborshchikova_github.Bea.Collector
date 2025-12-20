using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class MessageQueueSender : IEventSender
    {
        public void Send(List<BillEvent> billEvents)
        {
            foreach (var billEvent in billEvents)
            {
                var billData = billEvent switch
                {
                    PaidBillEvent paid => JsonSerializer.Serialize(paid),
                    CancelledBillEvent cancelled => JsonSerializer.Serialize(cancelled)
                };
                var sendEvent = new SendEvent(billEvent.Id, billEvent.OperationDateTime,
                    billEvent.UserId, billEvent.EventType, billData);

                const int maxRetries = 3;
                for (var attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        Thread.Sleep(20);
                        return;
                    }
                    catch
                    {
                        if (attempt != maxRetries)
                        {
                            Thread.Sleep(1000 * attempt);
                        }
                    }
                }
            }
        }
    }
}

