using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class TaskProcessor : IProcessor
    {
        private readonly ILogger<TaskProcessor> _logger;
        private readonly ISendEventLogRepository _sendLogRepository;
        private readonly IEventSender _dbSender;

        public TaskProcessor(IEventSender dbSender
            , ILogger<TaskProcessor> logger, ISendEventLogRepository sendLogRepository)
        {
            _dbSender = dbSender;
            _logger = logger;
            _sendLogRepository = sendLogRepository;
        }

        public async Task<bool> ProcessAsync(List<EventProcessRange> ranges)
        {
            var isSendCompleted = true;
            var tasks = ranges.Select(async range =>
            {
                try
                {
                    //if (range.Id == 2) //TODO: удалить перед завершением PR по BackgroundTask
                    //    throw new Exception("Диапазон номер 2");
                    await _dbSender.SendAsync(range);
                }
                catch (Exception ex)
                {
                    isSendCompleted = false;
                    _logger.LogError(ex, $"Информация об ошибке в " +
                    $"Task Id={Task.CurrentId} при обработке RangeId={range.Id}");
                }
            });

            await Task.WhenAll(tasks);
            return isSendCompleted;
            //SaveSendResult(isSendCompleted, processingContext);
        }

        //private void SaveSendResult(bool isSendCompleted, ProcessingContext processingContext)
        //{
        //    var utcRunDateTime = DateTime.SpecifyKind(processingContext.RunDateTime, DateTimeKind.Utc);
        //    var fileName = Path.GetFileName(processingContext.FileName);
        //    var workerServiceSendLog = new WorkerServiceSendLog(fileName, utcRunDateTime
        //        , processingContext.RunSettings, isSendCompleted);
        //    _sendLogRepository.SaveSendResult(workerServiceSendLog);
        //}
    }
}
