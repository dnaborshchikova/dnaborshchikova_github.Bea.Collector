using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Diagnostics;

namespace dnaborshchikova_github.Bea.Collector.Processor.Handlers
{
    public class EventProcessor : IEventProcessor
    {
        private readonly Func<string, IProcessor> _processor;
        private readonly IParcer _parcer;

        public EventProcessor(Func<string, IProcessor> processor, IParcer parcer)
        {
            _parcer = parcer;
            _processor = processor;
        }

        public void Process(string filePath, int threadCount, string processType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"Начата обработка файла.");

            var billEvents = _parcer.Parse(filePath).OrderBy(e => e.OperationDateTime).ToList();
            var ranges = GenerateParts(billEvents, threadCount);

            var processor = _processor(processType);
            processor.Process(ranges);
            stopwatch.Stop();
            Console.WriteLine($"Завершена обработка файла.\nКоличество потоков {threadCount}, время обработки {stopwatch.ElapsedMilliseconds} мс.");
        }

        private List<List<BillEvent>> GenerateParts(List<BillEvent> billEvents, int threadCount)
        {
            var dates = billEvents.Select(e => e.OperationDateTime.Date).ToList();
            var minDate = dates.Min(e => e.Date);
            var maxDate = dates.Max(e => e.Date);
            var total = maxDate - minDate;
            var step = TimeSpan.FromTicks(total.Ticks / threadCount);

            var dateRanges = new List<DateTime>();
            for (int i = 0; i <= threadCount; i++)
                dateRanges.Add(minDate + TimeSpan.FromTicks(step.Ticks * i));

            var eventRanges = new List<List<BillEvent>>();

            for (int i = 0; i < dateRanges.Count; i++)
            {
                var nextRangeIndex = i + 1;
                var eventRange = new List<BillEvent>();
                if (nextRangeIndex != dateRanges.Count)
                {
                    eventRange = billEvents.Where(e => e.OperationDateTime.Date >= dateRanges[i]
                                            && e.OperationDateTime.Date < dateRanges[nextRangeIndex]).ToList();
                }
                else
                {
                    eventRange = billEvents.Where(e => e.OperationDateTime.Date >= dateRanges[i]).ToList();
                }

                eventRanges.Add(eventRange);
            }

            return eventRanges;
        }
    }
}
