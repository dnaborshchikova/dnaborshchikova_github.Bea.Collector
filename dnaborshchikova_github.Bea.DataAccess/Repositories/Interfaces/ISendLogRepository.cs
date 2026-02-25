using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces
{
    public interface ISendLogRepository
    {
        void SaveSendResult(SendEventLog sendEventLog);
        SendEventLog IsPreviousDaySendComplete(string filePath);
    }
}
