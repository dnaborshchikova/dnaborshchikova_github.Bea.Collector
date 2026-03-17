using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Repositories
{
    public class SendEventLogRepository : ISendEventLogRepository
    {
        private readonly CollectorDbContext _collectorDbContext;

        public SendEventLogRepository(CollectorDbContext collectorDbContext)
        {
            _collectorDbContext = collectorDbContext;
        }

        public async Task SaveSendResultAsync(SendEventLog sendEventLog)
        {
            _collectorDbContext.SendEventLogs.Add(sendEventLog);
            await _collectorDbContext.SaveChangesAsync();
        }

        public SendEventLog IsPreviousDaySendComplete(string fileName)
        {
            return _collectorDbContext.SendEventLogs
                  .Where(l => l.FileName == fileName)
                  .OrderByDescending(l => l.RunDateTime)
                  .ToList()
                  .FirstOrDefault();
        }
    }
}
