using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IEventSender
    {
        public void Send(List<BillEvent> billEvents);
    }
}
