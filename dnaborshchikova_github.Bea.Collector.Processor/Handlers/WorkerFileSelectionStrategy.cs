using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class WorkerFileSelectionStrategy : IFileSelectionStrategy
    {
        private readonly IWorkerServiceLogRepository _workerServiceLogRepository;

        public WorkerFileSelectionStrategy(IWorkerServiceLogRepository workerServiceLogRepository)
        {
            _workerServiceLogRepository = workerServiceLogRepository;
        }

        public List<string> GetFiles()
        {
            var currentDate = GetCurrentDate();
            var filePaths = new List<string>();
            var previuosDayFileName = GetFileName(currentDate.AddDays(-1));
            var previuosDaySendLog = _workerServiceLogRepository.IsPreviousDaySendComplete(previuosDayFileName);
            if (previuosDaySendLog != null && !previuosDaySendLog.IsSendCompleted)
            {
                var filePath = BuildFullPath(previuosDayFileName);
                if (IsFileExists(filePath))
                {
                    filePaths.Add(filePath);
                }
            }
            var currentDayFileName = GetFileName(currentDate);
            var currentDayFilePath = BuildFullPath(currentDayFileName);
            if (IsFileExists(currentDayFilePath))
            {
                filePaths.Add(currentDayFilePath);
            }

            return filePaths;
        }

        private string BuildFullPath(string fileName)
        {
            var folderPath = "C:\\"; //TODO: брать путь из конфига
            var filePath = Path.Combine(folderPath, fileName);

            return filePath;
        }

        private string GetFileName(DateTime date)
        {
            return $"{date.ToShortDateString()}_BillEvent.csv"; //TODO: брать расширение из конфига
        }

        private bool IsFileExists(string filePath)
        {
            var isFileExists = File.Exists(filePath);
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException($"File {filePath} does not exist.");
            }

            return isFileExists;
        }

        private DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
    }
}
