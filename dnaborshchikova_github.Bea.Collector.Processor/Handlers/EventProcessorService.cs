using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class EventProcessorService : IEventProcessor
    {
        private readonly AppSettings _appSettings;
        private readonly Func<string, IProcessor> _processor;
        private readonly IParcer _parcer;
        private readonly ILogger<EventProcessorService> _logger;

        public EventProcessorService(Func<string, IProcessor> processor, IParcer parcer
            , AppSettings appSettings, ILogger<EventProcessorService> logger)
        {
            _parcer = parcer;
            _processor = processor;
            _appSettings = appSettings;
            _logger = logger;
        }

        public void Process()
        {
            Console.WriteLine($"File processing started.");
            _logger.LogInformation($"Processing start {DateTime.Now}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var filePath = _appSettings.ProcessingSettings.FilePath;
            _logger.LogInformation($"Parse file start. File path: {filePath}");

            var billEvents = _parcer.Parse(filePath).OrderBy(e => e.OperationDateTime).ToList();
            var ranges = GenerateParts(billEvents, _appSettings.ProcessingSettings.ThreadCount);
            var processor = _processor(_appSettings.ProcessingSettings.ProcessType);
            processor.Process(ranges);

            stopwatch.Stop();
            _logger.LogInformation($"Processing end {DateTime.Now}. Total processing time: {stopwatch.ElapsedMilliseconds} ms.");
            Console.WriteLine($"File processing completed.");
        }

        public List<EventProcessRange> GenerateParts(List<BillEvent> billEvents, int threadCount)
        {
            _logger.LogInformation($"Fill ranges start.");

            var dateRanges = GetDataRanges(billEvents, threadCount);
            var eventRanges = new List<EventProcessRange>();
            for (int i = 0; i < dateRanges.Count; i++)
            {
                var nextRangeIndex = i + 1;
                var events = new List<BillEvent>();
                if (nextRangeIndex < dateRanges.Count)
                {
                    events = billEvents.Where(e => e.OperationDateTime.Date >= dateRanges[i]
                                            && e.OperationDateTime.Date < dateRanges[nextRangeIndex]).ToList();
                }
                else
                {
                    events = billEvents.Where(e => e.OperationDateTime.Date >= dateRanges[i]).ToList();
                }

                var range = new EventProcessRange(i + 1, events);
                eventRanges.Add(range);
                _logger.LogInformation($"Filling range end. Range id: {range.Id}. Events count: {events.Count}.");
            }

            _logger.LogInformation($"Fill ranges end.");
            return eventRanges;
        }

        private List<DateTime> GetDataRanges(List<BillEvent> billEvents, int threadCount)
        {
            _logger.LogInformation($"Generating ranges start. Event count {billEvents.Count}. Thread count {threadCount}");

            var dates = billEvents.Select(e => e.OperationDateTime.Date).ToList();
            var minDate = dates.Min(e => e.Date);
            var maxDate = dates.Max(e => e.Date);
            var total = maxDate - minDate;
            var step = TimeSpan.FromTicks(total.Ticks / threadCount);
            var dateRanges = new List<DateTime>();

            if (threadCount != 1)
            {
                dateRanges = new List<DateTime>();
                for (int i = 0; i < threadCount; i++)
                    dateRanges.Add(minDate + TimeSpan.FromTicks(step.Ticks * i));
            }
            else
            {
                dateRanges = new List<DateTime> { minDate };
            }

            _logger.LogInformation($"Generating ranges end. Range count: {dateRanges.Count}.");
            return dateRanges;
        }
    }
}
