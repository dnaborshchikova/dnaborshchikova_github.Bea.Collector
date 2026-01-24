using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;

namespace dnaborshchikova_github.Bea.Collector.Processor.Services
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

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessAsync();
                await Task.Delay(1000, cancellationToken);
            }
        }

        public async Task ProcessAsync()
        {
            _logger.LogInformation($"Start processing {DateTime.Now}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var filePath = _appSettings.ProcessingSettings.FilePath;
                _logger.LogInformation($"Parse file. File path: {filePath}");
                var billEvents = _parser.Parse(filePath).OrderBy(e => e.OperationDateTime).ToList();

                _logger.LogInformation($"Start generate event ranges.");
                var ranges = RangeGenerator.GenerateParts(billEvents, _appSettings.ProcessingSettings.ThreadCount);
                ranges.ForEach(r => _logger.LogInformation($"Generate event range end. Range id: {r.Id}." +
                    $"Events count: {r.BillEvents.Count}."));
                _logger.LogInformation($"End generate event ranges.");

                var processor = _processor(_appSettings.ProcessingSettings.ProcessType);
                await processor.ProcessAsync(ranges);
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
    }
}
