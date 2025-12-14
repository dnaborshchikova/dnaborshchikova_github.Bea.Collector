using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class ThreadProcessor : IProcessor
    {
        private readonly IEventSender _eventSender;

        public ThreadProcessor(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }

        public void Process(List<List<BillEvent>> ranges)
        {
            foreach (var range in ranges)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        Send(range);
                    }
                    catch (Exception ex)
                    {
                        //Log.Error(ex, "Ошибка в потоке обработки диапазона");
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
