using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface ICompositeEventSender
    {
        SendEvent Send(EventProcessRange range);
        Task<SendEvent> SendAsync(EventProcessRange range);
    }
}
