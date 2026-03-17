using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.DataAccess;
using System.Threading;

namespace dnaborshchikova_github.Bea.Collector.WorkerService.Services
{
    public class PeriodicHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PeriodicHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope(); 
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>(); 


            var interval = TimeSpan.FromHours(24);
            while (!cancellationToken.IsCancellationRequested)
            {
                await eventProcessor.ProcessAsync();
                await Task.Delay(interval, cancellationToken);
            }
        }
    }
}
