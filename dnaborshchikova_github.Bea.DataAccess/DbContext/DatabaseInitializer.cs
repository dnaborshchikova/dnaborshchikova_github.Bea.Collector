using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.DbContext
{
    public class DatabaseInitializer
    {
        private readonly CollectorDbContext _context;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(CollectorDbContext context, ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void CreateDatabase()
        {
            if (_context.Database.CanConnect()) //TODO: закомментировано для запуска в режиме сервиса.
            {
                _context.Database.EnsureDeleted();
                _logger.LogInformation("Database deleted.");
            }

            if (!_context.Database.CanConnect())
            {
                _context.Database.EnsureCreated();
                _logger.LogInformation("Database created.");
            }
        }
    }
}
