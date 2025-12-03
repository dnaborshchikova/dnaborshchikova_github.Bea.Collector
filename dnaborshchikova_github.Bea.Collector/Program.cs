using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

if (!bool.TryParse(config["GenerateFile"], out var isGenerateFile))
    throw new Exception($"Конфигурационный файл не настроен. Не задан параметр isGenerateFile.");

if (!isGenerateFile)
{
    var filePath = config["FilePath"];
    if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentException("Конфигурационный файл не настроен." +
            "Не указан путь к файлу. Укажите путь или поставьте значение false для параметра GenerateFile.");
    return;
}

if (!bool.TryParse(config["RunAsProcess"], out var isRunAsProcess))
    throw new Exception($"Конфигурационный файл не настроен. Не задан параметр RunAsProcess.");

if (isRunAsProcess)
{
    var proccess = new Process();
    proccess.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "dnaborshchikova_github.Bea.Generator.exe"); ;
    proccess.Start();
    return;
}

var errors = new List<string>();
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

Console.ReadLine();