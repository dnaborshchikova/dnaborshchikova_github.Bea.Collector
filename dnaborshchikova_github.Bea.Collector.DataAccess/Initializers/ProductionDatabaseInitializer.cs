using dnaborshchikova_github.Bea.Collector.DataAccess.Initializers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.DataAccess.Initializers
{
    public class ProductionDatabaseInitializer : IDatabaseInitializer
    {
        private readonly CollectorDbContext _context;
        private readonly ILogger<ProductionDatabaseInitializer> _logger;

        public ProductionDatabaseInitializer(CollectorDbContext context
            , ILogger<ProductionDatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                _context.Database.Migrate();
                _logger.LogInformation("Database migrated (prod).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error migrating production database.");
                throw;
            }
        }
    }
}
