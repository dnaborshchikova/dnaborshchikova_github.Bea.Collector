using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Collector.Sender
{
    public class EventReadCheckpointRepository : IEventReadCheckpointRepository
    {
        private readonly CollectorDbContext _dbContext;

        public EventReadCheckpointRepository(CollectorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SetLastEventReadCheckpoint(EventReadCheckpoint eventReadCheckpoint)
        {
            _dbContext.EventReadCheckpoints.Add(eventReadCheckpoint);
            _dbContext.SaveChanges();
        }
    }
}
