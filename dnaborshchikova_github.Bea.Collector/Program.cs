using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
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

#region Валидация запуска

var errors = new List<string>();
if (!bool.TryParse(config["GenerateFile"], out var isGenerateFile))
    errors.Add($"Не задан параметр isGenerateFile.");

var threadCount = 0;
if (!int.TryParse(config["ThreadCount"], out threadCount))
    errors.Add($"Не задан параметр ThreadCount.");

var processType = config["ProcessType"];
if (string.IsNullOrEmpty(processType))
    errors.Add("Не указан путь к файлу. Укажите путь или поставьте значение false для параметра ProcessType.");

if (errors.Count > 0)
{
    throw new Exception($"Конфигурационный файл не настроен." +
        $"Список ошибок:\nНе задан параметры {string.Concat("\n", errors)}\n");
}

var filePath = string.Empty;
if (!isGenerateFile)
{
    filePath = config["FilePath"];
    if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentException("Конфигурационный файл не настроен." +
            "Не указан путь к файлу. Укажите путь или поставьте значение false для параметра GenerateFile.");
}
else
{
    if (!bool.TryParse(config["RunAsProcess"], out var isRunAsProcess))
        throw new Exception($"Конфигурационный файл не настроен. Не задан параметр RunAsProcess.");

    if (isRunAsProcess)
    {
        var proccess = new System.Diagnostics.Process();
        filePath = Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now.ToShortDateString()}_BillEvent.csv");
        proccess.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, $"dnaborshchikova_github.Bea.Generator.exe");
        proccess.Start();
        proccess.WaitForExit();
    }
    else
    {
        errors = new List<string>();
        if (!int.TryParse(config["PaidBillEventCount"], out var paidBillEventCount))
            errors.Add("PaidBillEventCount");

        if (!int.TryParse(config["CancelledBillEventCount"], out var cancelledBillEventCount))
            errors.Add("CancelledBillEventCount");

        var fileFormat = config["FileFormat"];
        if (string.IsNullOrWhiteSpace(fileFormat))
            errors.Add("FileFormat");

        if (errors.Count > 0)
            throw new Exception($"Конфигурационный файл не настроен.\nНе заданы параметры:\n {string.Join("\n", errors)}");

        var runner = new AppRunner();
        runner.Generate(fileFormat, paidBillEventCount, cancelledBillEventCount);
    }
}
#endregion

var host = Host.CreateDefaultBuilder().ConfigureServices(services =>
    {
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

var generator = host.Services.GetService<IEventProcessor>();
generator.Process(filePath, threadCount, processType);
Console.ReadLine();