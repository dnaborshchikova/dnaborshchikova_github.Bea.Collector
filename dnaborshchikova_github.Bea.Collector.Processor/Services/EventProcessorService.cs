using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Processor.Services
{
    public class EventProcessorService : IEventProcessor
    {
        private readonly AppSettings _appSettings;
        private readonly Func<string, IProcessor> _processor;
        private readonly IParser _parser;
        private readonly ILogger<EventProcessorService> _logger;
        private IWorkerServiceLogRepository _workerServiceLogRepository;

        public EventProcessorService(Func<string, IProcessor> processor, IParser parser
            , AppSettings appSettings, ILogger<EventProcessorService> logger
            , IWorkerServiceLogRepository workerServiceLogRepository)
        {
            _parser = parser;
            _processor = processor;
            _appSettings = appSettings;
            _logger = logger;
            _workerServiceLogRepository = workerServiceLogRepository;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var interval = TimeSpan.FromHours(24); //TODO: получать из конфига
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessAsync();
                await Task.Delay(interval, cancellationToken);
            }
        }

        public async Task ProcessAsync()
        {
            var currentDate = GetCurrentDate();
            _logger.LogInformation($"Start processing {DateTime.Now}.");
            
            var previuosDayFileName = GetFileName(currentDate.AddDays(-1));
            var previuosDaySendLog = _workerServiceLogRepository.IsPreviousDaySendComplete(previuosDayFileName);
            if (previuosDaySendLog != null && !previuosDaySendLog.IsSendCompleted)
            {
                var filePath = BuildFullPath(previuosDayFileName);
                if (IsFileExists(filePath))
                {
                    await ProcessFile(filePath);
                }
            }
            var currentDayFileName = GetFileName(currentDate);
            var currentDayFilePath = BuildFullPath(currentDayFileName);
            if (IsFileExists(currentDayFilePath))
            {
                await ProcessFile(currentDayFilePath);
            }
        }

        private async Task ProcessFile(string filePath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                _logger.LogInformation($"Parse file. File path: {filePath}");
                var billEvents = _parser.Parse(filePath).OrderBy(e => e.OperationDateTime).ToList();

                _logger.LogInformation($"Start generate event ranges.");
                var ranges = RangeGenerator.GenerateParts(billEvents, _appSettings.ProcessingSettings.ThreadCount);
                ranges.ForEach(r => _logger.LogInformation($"Generate event range end. Range id: {r.Id}." +
                    $"Events count: {r.BillEvents.Count}."));
                _logger.LogInformation($"End generate event ranges.");

                var processor = _processor(_appSettings.ProcessingSettings.ProcessType);
                var runSettings = JsonSerializer.Serialize(_appSettings);
                var currentDate = GetCurrentDate();
                var processingContext = new ProcessingContext(filePath, currentDate, runSettings);
                await processor.ProcessAsync(ranges, processingContext);
            }
            catch (Exception ex)
            {
                throw new ProcessingException("Error process file.", ex);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"End processing {DateTime.Now}." +
                    $"Total processing time: {stopwatch.ElapsedMilliseconds} ms.");
            }
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
