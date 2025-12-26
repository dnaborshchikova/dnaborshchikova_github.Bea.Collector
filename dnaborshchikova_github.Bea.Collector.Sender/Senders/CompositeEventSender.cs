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

        public void Send(EventProcessRange range)
        {
            foreach (var sender in _senders)
            {
                sender.Send(range);
            }
        }
    }
}
