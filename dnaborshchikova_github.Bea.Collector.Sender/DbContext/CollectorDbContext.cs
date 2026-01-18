using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace dnaborshchikova_github.Bea.Collector.Sender
{
    public class CollectorDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public CollectorDbContext(DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<SendEvent> SendEvents { get; set; }
        public DbSet<EventReadCheckpoint> EventReadCheckpoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CollectorDbContext).Assembly);
        }
    }
}
