using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class TaskProcessor : IProcessor
    {
        private readonly ICompositeEventSender _compositeEventSender;
        private readonly ILogger<TaskProcessor> _logger;

        public TaskProcessor(ICompositeEventSender compositeEventSender
            , ILogger<TaskProcessor> logger)
        {
            _compositeEventSender = compositeEventSender;
            _logger = logger;
        }

        public async Task<SendEvent?> ProcessAsync(List<EventProcessRange> ranges)
        {
            var sendEvents = new List<SendEvent>();
            var tasks = ranges.Select(async range =>
            {
                try
                {
                    var lastSendEvent = await _compositeEventSender.SendAsync(range);
                    sendEvents.Add(lastSendEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Информация об ошибке в " +
                    $"Task Id={Task.CurrentId} при обработке RangeId={range.Id}");
                }
            });
            await Task.WhenAll(tasks);
            var lastSendEvent = sendEvents
                .OrderByDescending(e => e.Date)
                .FirstOrDefault();

            return lastSendEvent;
        }
    }
}
