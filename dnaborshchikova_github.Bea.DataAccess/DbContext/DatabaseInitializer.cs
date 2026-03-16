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
            try
            {
                // Удаляем базу (если она существует)
                _context.Database.EnsureDeleted();
                _logger.LogInformation("Database deleted.");

                // Создаём базу и все таблицы заново
                _context.Database.EnsureCreated();
                _logger.LogInformation("Database created with tables.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating database.");
                throw;
            }
        }
    }
}
