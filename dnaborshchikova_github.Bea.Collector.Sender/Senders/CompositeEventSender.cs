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

        public async Task SendAsync(EventProcessRange range)
        {
            var tasks = new List<Task>();
            foreach (var sender in _senders)
            {
                var taskk = sender.SendAsync(range);
                tasks.Add(taskk);
                //tasks = _senders.Select(sender => sender.SendAsync(range)).ToList();
            }
            //tasks = _senders.Select(sender => sender.SendAsync(range)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
