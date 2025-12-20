using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Processor.Processors
{
    public class ThreadProcessor : IProcessor
    {
        private readonly IEventSender _eventSender;
        private readonly ILogger<ThreadProcessor> _logger;

        public ThreadProcessor(IEventSender eventSender, ILogger<ThreadProcessor> logger)
        {
            _eventSender = eventSender;
            _logger = logger;
        }

        public void Process(List<EventProcessRange> ranges)
        {
            using var countdown = new CountdownEvent(ranges.Count);

            foreach (var range in ranges)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        _eventSender.Send(range);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, "Ошибка в потоке обработки диапазона.");
                    }
                    finally
                    {
                        countdown.Signal();
                    }
                });

                thread.Start();
                
            }

            countdown.Wait();
        }
    }
}
