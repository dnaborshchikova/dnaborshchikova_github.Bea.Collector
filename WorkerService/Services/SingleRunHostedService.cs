using dnaborshchikova_github.Bea.Collector.Core.Interfaces;

namespace dnaborshchikova_github.Bea.Collector.WorkerService.Services
{
    public class SingleRunHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SingleRunHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
            await processor.ProcessAsync();
        }
    }
}
