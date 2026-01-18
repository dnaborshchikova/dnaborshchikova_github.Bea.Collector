using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class CompositeEventSender : ICompositeEventSender
    {
        private readonly IEnumerable<IEventSender> _senders;

        public CompositeEventSender(IEnumerable<IEventSender> senders)
        {
            _senders = senders;
        }

        public SendEvent Send(EventProcessRange range)
        {
            var lastSendEvents = new List<SendEvent>();
            foreach (var sender in _senders)
            {
                var sendEvent = sender.Send(range);
                lastSendEvents.Add(sendEvent);
            }

            return lastSendEvents.SingleOrDefault(); //TODO переделать с учетом нескольких серндеров
        }

        public async Task<SendEvent> SendAsync(EventProcessRange range)
        {
            var tasks = _senders.Select(sender => sender.SendAsync(range)).ToList();
            await Task.WhenAll(tasks);
            var result = tasks.Select(t => t.Result).ToList();

            return result.SingleOrDefault(); //TODO переделать с учетом нескольких серндеров
        }
    }
}
