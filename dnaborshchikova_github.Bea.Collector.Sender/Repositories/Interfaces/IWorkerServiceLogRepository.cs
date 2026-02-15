using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Sender.Repositories.Interfaces
{
    public interface IWorkerServiceLogRepository
    {
        void SaveSendResult(WorkerServiceSendLog workerServiceSendLog);
        WorkerServiceSendLog IsPreviousDaySendComplete(string filePath);
    }
}
