using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

        public SendEvent Send(EventProcessRange range)
        {
            _logger.LogInformation($"Start save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. " +
                $"Thread id: {Thread.CurrentThread.ManagedThreadId}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            SendEvent lastEvent = null;
            using var dbContext = _contextFactory.CreateDbContext();
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            using var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            try
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
                    dbContext.SaveChanges();

                    if (lastEvent == null || sendEvent.Date > lastEvent.Date)
                    {
                        lastEvent = sendEvent;
                    }
                }
                dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }

            stopwatch.Stop();
            _logger.LogInformation($"End save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. "
                + $"Thread id: {Thread.CurrentThread.ManagedThreadId}. "
                + $"Work time: {stopwatch.ElapsedMilliseconds} ms.");
            return lastEvent;
        }

        public async Task<SendEvent> SendAsync(EventProcessRange range)
        {
            _logger.LogInformation($"Start save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. " +
               $"Thread id: {Thread.CurrentThread.ManagedThreadId}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using var dbContext = _contextFactory.CreateDbContext();
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            SendEvent lastEvent = null;
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
                await dbContext.SaveChangesAsync();
                dbContext.ChangeTracker.Clear();

                if (lastEvent == null || sendEvent.Date > lastEvent.Date)
                {
                    lastEvent = sendEvent;
                }
            }
            await dbContext.SaveChangesAsync();

            stopwatch.Stop();
            _logger.LogInformation($"End save events. Range id: {range.Id}. Event count: {range.BillEvents.Count}. "
                + $"Thread id: {Thread.CurrentThread.ManagedThreadId}. "
                + $"Work time: {stopwatch.ElapsedMilliseconds} ms.");
            return lastEvent;
        }
    }
}
