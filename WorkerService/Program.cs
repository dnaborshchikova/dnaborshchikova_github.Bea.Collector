using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Services;
using dnaborshchikova_github.Bea.Collector.DataAccess;
using dnaborshchikova_github.Bea.Collector.DataAccess.DbContext;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using dnaborshchikova_github.Bea.Collector.Parser.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Processors;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using dnaborshchikova_github.Bea.Collector.Sender.Handlers;
using dnaborshchikova_github.Bea.Collector.WorkerService.Models;
using dnaborshchikova_github.Bea.Collector.WorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

var builder = Host.CreateApplicationBuilder(args);

// Конфигурация
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Update"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.ChangeTracking"))
    .CreateLogger();

// Очистка и добавление провайдеров логирования
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Чтение настроек из конфигурации
        var config = hostContext.Configuration;
        var generatorSettings = config.GetSection(nameof(GeneratorSettings)).Get<GeneratorSettings>();
        var processingSettings = config.GetSection(nameof(ProcessingSettings)).Get<ProcessingSettings>();
        var workerServiceSettings = config.GetSection(nameof(WorkerServiceSettings)).Get<WorkerServiceSettings>();

        var appSettingsService = new AppSettingsService();
        var appSettings = appSettingsService.CreateAppSettings(generatorSettings, processingSettings);

        // Регистрация зависимостей
        services.AddSingleton(appSettings);

        services.AddScoped<DatabaseInitializer>();
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

        services.AddScoped<IEventSender, DataBaseSender>();
        services.AddScoped<IParser, CsvParser>();
        services.AddScoped<IEventProcessor, EventProcessorService>();
        services.AddScoped<ISendEventLogRepository, SendEventLogRepository>();
        services.AddScoped<IFileSelectionStrategy, WorkerFileSelectionStrategy>();

        // Настройка подключения к базе данных
        services.AddDbContextFactory<CollectorDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("Default"));
        });

        // Условия для разных режимов работы
        if (appSettings.ProcessingSettings.RunMode == "Worker")
            services.AddHostedService<SingleRunHostedService>();
        if (appSettings.ProcessingSettings.RunMode == "Service")
            services.AddHostedService<PeriodicHostedService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    databaseInitializer.CreateDatabase();
}
host.Run();