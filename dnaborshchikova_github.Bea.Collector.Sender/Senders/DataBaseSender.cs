using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class DataBaseSender : IEventSender
    {
        private readonly DbContextOptions<CollectorDbContext> _options;

        public DataBaseSender(DbContextOptions<CollectorDbContext> options)
        {
            _options = options;
        }

        public void Send(EventProcessRange range)
        {
            using (var dbContext = new CollectorDbContext(_options))
            {
                foreach (var billEvent in range.BillEvents)
                {
                    var billData = billEvent switch
                    {
                        PaidBillEvent paid => JsonSerializer.Serialize(paid),
                        CancelledBillEvent cancelled => JsonSerializer.Serialize(cancelled)
                    };
                    var utcTime = DateTime.SpecifyKind(billEvent.OperationDateTime, DateTimeKind.Utc);

                    var sendEvent = new SendEvent(billEvent.Id, utcTime,
                        billEvent.UserId, billEvent.EventType, billData);

                    dbContext.SendEvents.Add(sendEvent);                    
                }
                dbContext.SaveChanges();
                Console.WriteLine(dbContext.SendEvents.Count());
            }
        }
    }
}
