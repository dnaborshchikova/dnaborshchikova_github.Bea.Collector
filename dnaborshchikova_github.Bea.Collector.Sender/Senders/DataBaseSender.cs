using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class DataBaseSender : IEventSender
    {
        private readonly ILogger<DataBaseSender> _logger;
        private readonly IDbContextFactory<CollectorDbContext> _contextFactory;

        public DataBaseSender(ILogger<DataBaseSender> logger
            , IDbContextFactory<CollectorDbContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public void Send(EventProcessRange range)
        {
            _logger.LogInformation($"Start save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}."); 

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using var dbContext = _contextFactory.CreateDbContext();
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
            stopwatch.Stop();

            _logger.LogInformation($"End save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. " +
                $"Work time: {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
