using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.DataAccess;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public Worker(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
            var dbContext = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
            dbContext.Database.EnsureCreated();

            await eventProcessor.ProcessAsync(cancellationToken);
        }
    }
}
