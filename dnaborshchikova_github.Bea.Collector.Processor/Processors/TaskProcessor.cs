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

        public async Task ProcessAsync(List<EventProcessRange> ranges)
        {
            var tasks = ranges.Select(async range =>
            {
                try
                {
                    await _compositeEventSender.SendAsync(range);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Информация об ошибке в " +
                    $"Task Id={Task.CurrentId} при обработке RangeId={range.Id}");
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
