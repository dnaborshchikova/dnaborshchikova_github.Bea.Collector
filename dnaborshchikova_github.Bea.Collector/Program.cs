using dnaborshchikova_github.Bea.Collector.App;
using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Parser.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Processors;
using dnaborshchikova_github.Bea.Collector.Sender;
using dnaborshchikova_github.Bea.Collector.Sender.DbContext;
using dnaborshchikova_github.Bea.Collector.Sender.Handlers;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var validator = new AppSettingsService(config);
var appSettings = validator.CreateAppSettings();

#region Запуск генератора
RunGenerator();
#endregion

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var host = Host.CreateDefaultBuilder()
     .ConfigureLogging(logging =>
     {
         logging.ClearProviders();
     })
    .ConfigureServices(services =>
    {
        services.AddSingleton(appSettings);
        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<ThreadProcessorWithLock>();
        //services.AddScoped<ThreadProcessor>();
        services.AddScoped<TaskProcessor>();
        services.AddScoped<Func<string, IProcessor>>(provider => key =>
        {
            return key switch
            {
                "Thread" => provider.GetRequiredService<ThreadProcessorWithLock>(),
                //"Thread" => provider.GetRequiredService<ThreadProcessor>(),
                "Task" => provider.GetRequiredService<TaskProcessor>()
            };
        });
        //services.AddScoped<IEventSender, MessageQueueSender>();
        services.AddScoped<IEventSender, DataBaseSender>();
        services.AddScoped<ICompositeEventSender, CompositeEventSender>();
        services.AddScoped<IParcer, CsvParser>();
        services.AddScoped<IEventProcessor, EventProcessorService>();
        services.AddDbContextFactory<CollectorDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("Default"));
        });
        services.AddDbContext<CollectorDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default")));
    })
    .UseSerilog()
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    databaseInitializer.CreateDatabase();
}

var eventProcessor = host.Services.GetService<IEventProcessor>();
eventProcessor.Process();
Console.ReadLine();

void RunGenerator()
{
    if (appSettings.ProcessingSettings.GeneratorRunAsProcess && appSettings.ProcessingSettings.GenerateFile)
    {
        var proccess = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "dnaborshchikova_github.Bea.Generator.exe"),
                WorkingDirectory = Path.Combine(AppContext.BaseDirectory),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        proccess.Start();
        var output = proccess.StandardOutput.ReadToEnd();
        var error = proccess.StandardError.ReadToEnd();
        proccess.WaitForExit();

        if (proccess.ExitCode != 0)
        {
            throw new Exception($"Generator failed: {error}");
        }

        appSettings.ProcessingSettings.FilePath = output.Trim();
    }
    else if (appSettings.ProcessingSettings.GenerateFile)
    {
        var runner = new AppRunner(appSettings.GeneratorSettings);
        appSettings.ProcessingSettings.FilePath = runner.Generate();
    }
}