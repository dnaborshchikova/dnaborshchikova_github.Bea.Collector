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

        public void Process(List<List<BillEvent>> ranges)
        {
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
                });

                thread.Start();
            }
        }

        private void Send(List<BillEvent> billEvents)
        {
            foreach (var billEvent in billEvents)
            {
                const int maxRetries = 3;

                for (var attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        _eventSender.Send(billEvent);
                        return;
                    }
                    catch
                    {
                        if (attempt != maxRetries)
                        {
                            Thread.Sleep(1000 * attempt);
                        }
                    }
                }
            }
        }
    }
}
