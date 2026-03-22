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
using dnaborshchikova_github.Bea.Collector.WorkerService.Validators;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

var builder = Host.CreateApplicationBuilder(args);

// Конфигурация
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;

// Чтение настроек из конфигурации
var section = config.GetSection(nameof(GeneratorSettings));
if (string.IsNullOrWhiteSpace(section["PaidBillEventCount"]))
    throw new InvalidOperationException("PaidBillEventCount не указан");
if (string.IsNullOrWhiteSpace(section["CancelledBillEventCount"]))
    throw new InvalidOperationException("CancelledBillEventCount не указан");
var generatorSettings = section.Get<GeneratorSettings>();

var processingSettingsSection = config.GetSection(nameof(ProcessingSettings));
if (string.IsNullOrWhiteSpace(processingSettingsSection["ThreadCount"]))
    throw new InvalidOperationException("ThreadCount не указан");
var processingSettings = processingSettingsSection.Get<ProcessingSettings>(); 

var workerServiceSettings = config.GetSection(nameof(WorkerServiceSettings)).Get<WorkerServiceSettings>();

var appSettingsValidator = new WorkerSettingsValidator();
appSettingsValidator.ValidateGeneratorSettings(generatorSettings);
appSettingsValidator.ValidateProcessingSettings(processingSettings);

var appSettingsService = new AppSettingsService();
var appSettings = appSettingsService.CreateAppSettings(generatorSettings, processingSettings);


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
        // Регистрация зависимостей
        services.AddSingleton(appSettings);
        services.AddSingleton(generatorSettings);

        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<ThreadProcessor>();
        services.AddScoped<TaskProcessor>();

        services.AddScoped<Func<string, IProcessor>>(provider => key =>
        {
            return key switch
            {
                "Thread" => provider.GetRequiredService<ThreadProcessor>(),
                "Task" => provider.GetRequiredService<TaskProcessor>(),
                "ThreadProcessorWithLock" => provider.GetRequiredService<ThreadProcessorWithLock>()
            };
        });

        services.AddScoped<IEventSender, DataBaseSender>();
        services.AddScoped<IParser, CsvParser>();
        services.AddScoped<IEventProcessor, EventProcessorService>();
        services.AddScoped<ISendEventLogRepository, SendEventLogRepository>();
        services.AddScoped<IFileSelectionStrategy, WorkerFileSelectionStrategy>();
        services.AddScoped<AppRunner>();

        // Настройка подключения к базе данных
        services.AddDbContextFactory<CollectorDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("Default"),
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null
                );
            });
        });

        // Условия для разных режимов работы
        if (appSettings.ProcessingSettings.RunMode == "OneTime")
            services.AddHostedService<SingleRunHostedService>();
        if (appSettings.ProcessingSettings.RunMode == "ScheduledService")
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

var appRunnerScope = host.Services.CreateScope();
var appRunner = appRunnerScope.ServiceProvider.GetRequiredService<AppRunner>();
var basePath = Path.Combine(AppContext.BaseDirectory, appSettings.ProcessingSettings.InputFolder);
appRunner.Generate(basePath);

host.Run();
