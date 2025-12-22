using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
