using dnaborshchikova_github.Bea.Collector.DataAccess.Initializers.Interfaces;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Initializers
{
    public class DevelopmentDatabaseInitializer : IDatabaseInitializer
    {
        private readonly CollectorDbContext _context;
        private readonly ILogger<DevelopmentDatabaseInitializer> _logger;

        public DevelopmentDatabaseInitializer(CollectorDbContext context
            , ILogger<DevelopmentDatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                _context.Database.EnsureDeleted();
                _logger.LogInformation("Database deleted (dev).");

                _context.Database.EnsureCreated();
                _logger.LogInformation("Database created with tables (dev).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing development database.");
                throw;
            }
        }
    }
}
