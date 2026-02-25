using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Repositories
{
    public class SendEventLogRepository : ISendEventLogRepository
    {
        private readonly CollectorDbContext _collectorDbContext;
        private readonly AppSettings _appSettings;

        public SendEventLogRepository(CollectorDbContext collectorDbContext, AppSettings appSettings)
        {
            _collectorDbContext = collectorDbContext;
            _appSettings = appSettings;
        }

        public void SaveSendResult(SendEventLog sendEventLog)
        {
            _collectorDbContext.SendEventLogs.Add(sendEventLog);
            _collectorDbContext.SaveChanges();
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
