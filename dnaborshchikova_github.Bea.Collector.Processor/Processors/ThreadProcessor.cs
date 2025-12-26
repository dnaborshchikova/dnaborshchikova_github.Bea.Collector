using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class ThreadProcessor : IProcessor
    {
        private readonly ICompositeEventSender _compositeEventSender;
        private readonly ILogger<ThreadProcessor> _logger;

        public ThreadProcessor(ICompositeEventSender compositeEventSender
            , ILogger<ThreadProcessor> logger)
        {
            _compositeEventSender = compositeEventSender;
            _logger = logger;
        }

        public void Process(List<EventProcessRange> ranges)
        {
            using var countdown = new CountdownEvent(ranges.Count);
            var exceptions = new ConcurrentQueue<Exception>();

            foreach (var range in ranges)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        _compositeEventSender.Send(range);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.InnerException, $"Ошибка в потоке обработки диапазона.");
                        exceptions.Enqueue(ex);
                    }
                    finally
                    {
                        countdown.Signal();
                    }
                });

                thread.Start();                
            }

            countdown.Wait();

            if (!exceptions.IsEmpty)
            {
                foreach (var ex in exceptions)
                {
                    _logger.LogInformation($"При обработке данных возникли ошибки:\n" +
                        $"{string.Format(";\n", ex.InnerException)}.");
                }
            }
        }
    }
}
