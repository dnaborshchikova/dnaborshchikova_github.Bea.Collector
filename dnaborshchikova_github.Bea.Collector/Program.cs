using dnaborshchikova_github.Bea.Collector.App;
using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Parser.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Sender.Handlers;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var validator = new AppSettingsService(config);
var appSettings = validator.CreateAppSettings();

#region Запуск генератора

if (appSettings.RunAsProcess && appSettings.GenerateFile)
{
    var proccess = new System.Diagnostics.Process();
    appSettings.FileFormat = Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now.ToShortDateString()}_BillEvent.csv");
    proccess.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, $"dnaborshchikova_github.Bea.Generator.exe");
    proccess.Start();
    proccess.WaitForExit();
}
else if (appSettings.GenerateFile)
{
    var runner = new AppRunner(appSettings);
    runner.Generate();
}

#endregion

var host = Host.CreateDefaultBuilder().ConfigureServices(services =>
    {
        services.AddSingleton(appSettings);
        services.AddScoped<ThreadProcessor>();
        services.AddScoped<TaskProcessor>();
        services.AddScoped<IParcer, CsvParser>();
        services.AddScoped<IEventProcessor, EventProcessor>();
        services.AddScoped<IEventSender, BillEventFileMQSender>();
        services.AddScoped<Func<string, IProcessor>>(provider => key =>
        {
            return key switch
            {
                "Thread" => provider.GetRequiredService<ThreadProcessor>(),
                "Task" => provider.GetRequiredService<TaskProcessor>()
            };
        });
    })
    .Build();

var eventProcessor = host.Services.GetService<IEventProcessor>();
eventProcessor.Process();
Console.ReadLine();