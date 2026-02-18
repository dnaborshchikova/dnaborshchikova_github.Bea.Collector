using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Repositories
{
    public class WorkerServiceLogRepository : IWorkerServiceLogRepository
    {
        private readonly CollectorDbContext _collectorDbContext;

        public WorkerServiceLogRepository(CollectorDbContext collectorDbContext)
        {
            _collectorDbContext = collectorDbContext;
        }

        public void SaveSendResult(WorkerServiceSendLog workerServiceSendLog)
        {
            _collectorDbContext.WorkerServiceSendLogs.Add(workerServiceSendLog);
            _collectorDbContext.SaveChanges();
        }

        public WorkerServiceSendLog IsPreviousDaySendComplete(string fileName)
        {
            return _collectorDbContext.WorkerServiceSendLogs
                  .Where(l => l.FileName == fileName)
                  .OrderByDescending(l => l.RunDateTime)
                  .ToList()
                  .FirstOrDefault();
        }
    }
}
