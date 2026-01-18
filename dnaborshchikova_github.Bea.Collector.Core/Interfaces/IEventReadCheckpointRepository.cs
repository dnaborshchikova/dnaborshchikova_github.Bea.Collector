using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IEventReadCheckpointRepository
    {
        void SetLastEventReadCheckpoint(EventReadCheckpoint eventReadCheckpoint);
    }
}
