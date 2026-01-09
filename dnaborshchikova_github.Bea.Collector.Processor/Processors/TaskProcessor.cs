using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

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
            var exceptions = new ConcurrentQueue<(int rangeId, int? taskId, Exception ex)>();
            var tasks = new List<Task>();

            foreach (var range in ranges)
            {
                try
                {
                    var task = _compositeEventSender.SendAsync(range);
                    tasks.Add(task);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error save events. Range id: {range.Id}. " +
                        $"Thread id: {Thread.CurrentThread.ManagedThreadId}. Error: {ex.Message}");
                    exceptions.Enqueue((range.Id, Thread.CurrentThread.ManagedThreadId, ex));
                }
            }
            await Task.WhenAll(tasks);

            if (!exceptions.IsEmpty)
            {
                foreach (var (rangeId, threadId, ex) in exceptions)
                {
                    _logger.LogError(ex, $"Подробная информация об ошибке в " +
                        $"Thread Id={Thread.CurrentThread.ManagedThreadId} при обработке RangeId={rangeId}");
                }
            }
        }
    }
}
