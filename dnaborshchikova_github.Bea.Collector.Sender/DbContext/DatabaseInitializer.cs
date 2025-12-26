using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Sender.DbContext
{
    public class DatabaseInitializer
    {
        private readonly CollectorDbContext _context;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(CollectorDbContext context)
        {
            _context = context;
        }

        public void CreateDatabase()
        {
            using (_context)
            {
                if (_context.Database.CanConnect())
                {
                    _context.Database.EnsureDeleted();
                    _logger.LogInformation("Database deleted.");
                }

                _context.Database.EnsureCreated();
                _logger.LogInformation("Database created.");
            }
        }
    }
}
