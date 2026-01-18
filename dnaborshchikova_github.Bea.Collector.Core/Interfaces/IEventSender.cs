using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IEventSender
    {
        public SendEvent Send(EventProcessRange range);
        public Task<SendEvent> SendAsync(EventProcessRange range);
    }
}
