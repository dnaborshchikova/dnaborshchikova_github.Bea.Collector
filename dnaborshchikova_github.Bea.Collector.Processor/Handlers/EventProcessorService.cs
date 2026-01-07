using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class EventProcessorService : IEventProcessor
    {
        private readonly AppSettings _appSettings;
        private readonly Func<string, IProcessor> _processor;
        private readonly IParser _parser;
        private readonly ILogger<EventProcessorService> _logger;

        public EventProcessorService(Func<string, IProcessor> processor, IParser parser
            , AppSettings appSettings, ILogger<EventProcessorService> logger)
        {
            _parser = parser;
            _processor = processor;
            _appSettings = appSettings;
            _logger = logger;
        }

        public void Process()
        {
            _logger.LogInformation($"Start processing {DateTime.Now}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var filePath = _appSettings.ProcessingSettings.FilePath;
            _logger.LogInformation($"Parse file. File path: {filePath}");

            try
            {
                var billEvents = _parser.Parse(filePath).OrderBy(e => e.OperationDateTime).ToList();
                var ranges = GenerateParts(billEvents, _appSettings.ProcessingSettings.ThreadCount);
                var processor = _processor(_appSettings.ProcessingSettings.ProcessType);
                processor.Process(ranges);
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

        public List<EventProcessRange> GenerateParts(List<BillEvent> billEvents, int threadCount)
        {
            _logger.LogInformation($"Start generate event ranges.");

            var dateRanges = GetDataRanges(billEvents, threadCount);
            var eventRanges = new List<EventProcessRange>();

            for (int i = 0; i < dateRanges.Count; i++)
            {
                var nextRangeIndex = i + 1;
                var events = new List<BillEvent>();
                if (nextRangeIndex < dateRanges.Count)
                {
                    events = billEvents.Where(e => e.OperationDateTime >= dateRanges[i]
                                            && e.OperationDateTime < dateRanges[nextRangeIndex]).ToList();
                }
                else
                {
                    events = billEvents.Where(e => e.OperationDateTime >= dateRanges[i]).ToList();
                }

                var range = new EventProcessRange(i + 1, events);
                eventRanges.Add(range);
                _logger.LogInformation($"Generate event range end. Range id: {range.Id}." +
                    $"Events count: {events.Count}.");
            }

            _logger.LogInformation($"End generate event ranges.");
            return eventRanges;
        }

        private List<DateTime> GetDataRanges(List<BillEvent> billEvents, int threadCount)
        {
            var dates = billEvents.Select(e => e.OperationDateTime).ToList();
            var minDate = dates.Min();
            var maxDate = dates.Max();
            var total = maxDate - minDate;
            var step = TimeSpan.FromTicks(total.Ticks / threadCount);
            var dateRanges = new List<DateTime>();

            if (threadCount != 1)
            {
                for (int i = 0; i < threadCount; i++)
                    dateRanges.Add(minDate + TimeSpan.FromTicks(step.Ticks * i));
            }
            else
            {
                dateRanges = new List<DateTime> { minDate };
            }

            _logger.LogInformation($"Generate date ranges end. Range count: {dateRanges.Count}.");
            return dateRanges;
        }
    }
}
