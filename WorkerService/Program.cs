using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Services;
using dnaborshchikova_github.Bea.Collector.DataAccess;
using dnaborshchikova_github.Bea.Collector.DataAccess.DbContext;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using dnaborshchikova_github.Bea.Collector.Parser.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Processors;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using dnaborshchikova_github.Bea.Collector.Sender.Handlers;
using dnaborshchikova_github.Bea.Collector.WorkerService.Models;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using System.Diagnostics;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;

var generatorSettings = config.GetSection(nameof(GeneratorSettings)).Get<GeneratorSettings>();
var processingSettings = config.GetSection(nameof(ProcessingSettings)).Get<ProcessingSettings>();
var workerServiceSettings = config.GetSection(nameof(WorkerServiceSettings)).Get<WorkerServiceSettings>();
var appSettingsService = new AppSettingsService();
var appSettings = appSettingsService.CreateAppSettings(generatorSettings, processingSettings);

#region Запуск генератора
//RunGenerator();
#endregion

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Update"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.ChangeTracking"))
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton(appSettings);
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<ThreadProcessor>();
builder.Services.AddScoped<TaskProcessor>();
builder.Services.AddScoped<Func<string, IProcessor>>(provider => key =>
{
    return key switch
    {
        "Thread" => provider.GetRequiredService<ThreadProcessor>(),
        "Task" => provider.GetRequiredService<TaskProcessor>()
    };
});
builder.Services.AddScoped<IEventSender, DataBaseSender>();
builder.Services.AddScoped<ICompositeEventSender, CompositeEventSender>();
builder.Services.AddScoped<IParser, CsvParser>();
builder.Services.AddScoped<IEventProcessor, EventProcessorService>();
builder.Services.AddScoped<IWorkerServiceLogRepository, WorkerServiceLogRepository>();
builder.Services.AddDbContextFactory<CollectorDbContext>(options =>
{
    options.UseNpgsql(config.GetConnectionString("Default"));
});
builder.Services.AddDbContext<CollectorDbContext>(options =>
{
    options.UseNpgsql(config.GetConnectionString("Default"));
});
var host = builder.Build();
host.Run();

#region Код запуска генератора
//void RunGenerator()
//{
//    if (appSettings.ProcessingSettings.GeneratorRunAsProcess && appSettings.ProcessingSettings.GenerateFile)
//    {
//        var proccess = new Process()
//        {
//            StartInfo = new ProcessStartInfo
//            {
//                FileName = Path.Combine(AppContext.BaseDirectory, "dnaborshchikova_github.Bea.Generator.exe"),
//                WorkingDirectory = Path.Combine(AppContext.BaseDirectory),
//                RedirectStandardOutput = true,
//                RedirectStandardError = true,
//                UseShellExecute = false,
//                CreateNoWindow = true
//            }
//        };
//        proccess.Start();
//        var output = proccess.StandardOutput.ReadToEnd();
//        var error = proccess.StandardError.ReadToEnd();
//        proccess.WaitForExit();

//        if (proccess.ExitCode != 0)
//        {
//            throw new Exception($"Generator failed: {error}");
//        }

//        appSettings.ProcessingSettings.FilePath = output.Trim();
//    }
//    else if (appSettings.ProcessingSettings.GenerateFile)
//    {
//        var runner = new AppRunner(appSettings.GeneratorSettings);
//        appSettings.ProcessingSettings.FilePath = runner.Generate();
//    }
//}
#endregion