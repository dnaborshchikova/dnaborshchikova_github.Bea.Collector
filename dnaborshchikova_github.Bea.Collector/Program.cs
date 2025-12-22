using dnaborshchikova_github.Bea.Collector.App;
using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Parser.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Processors;
using dnaborshchikova_github.Bea.Collector.Sender;
using dnaborshchikova_github.Bea.Collector.Sender.Handlers;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var validator = new AppSettingsService(config);
var appSettings = validator.CreateAppSettings();

#region Запуск генератора

if (appSettings.ProcessingSettings.RunAsProcess && appSettings.ProcessingSettings.GenerateFile)
{
    var proccess = new System.Diagnostics.Process();
    appSettings.GeneratorSettings.FileFormat = Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now.ToShortDateString()}_BillEvent.csv");
    proccess.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, $"dnaborshchikova_github.Bea.Generator.exe");
    proccess.Start();
    proccess.WaitForExit();
}
else if (appSettings.ProcessingSettings.GenerateFile)
{
    var runner = new AppRunner(appSettings);
    runner.Generate();
}

#endregion

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var host = Host.CreateDefaultBuilder().ConfigureServices(services =>
    {
        services.AddSingleton(appSettings);
        services.AddScoped<ThreadProcessor>();
        services.AddScoped<TaskProcessor>();
        services.AddScoped<Func<string, IProcessor>>(provider => key =>
        {
            return key switch
            {
                "Thread" => provider.GetRequiredService<ThreadProcessor>(),
                "Task" => provider.GetRequiredService<TaskProcessor>()
            };
        });
        //services.AddScoped<IEventSender, MessageQueueSender>();
        services.AddScoped<IEventSender, DataBaseSender>();
        services.AddScoped<ICompositeEventSender, CompositeEventSender>();
        services.AddScoped<IParcer, CsvParser>();
        services.AddScoped<IEventProcessor, EventProcessorService>();
        services.AddScoped<ILogger, Logger>();
        services.AddDbContext<CollectorDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default")));
    })
    .UseSerilog()
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
    dbContext.Database.Migrate();
}

var eventProcessor = host.Services.GetService<IEventProcessor>();
eventProcessor.Process();
Console.ReadLine();