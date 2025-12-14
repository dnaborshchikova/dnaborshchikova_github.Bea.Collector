using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Генератор запущен.");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var paidBillEventCount = 0;
var cancelledBillEventCount = 0;
var fileFormat = string.Empty;

var errors = new List<string>();
if (!int.TryParse(config["PaidBillEventCount"], out paidBillEventCount))
    errors.Add("PaidBillEventCount");

if (!int.TryParse(config["CancelledBillEventCount"], out cancelledBillEventCount))
    errors.Add("CancelledBillEventCount");

fileFormat = config["FileFormat"];
if (string.IsNullOrWhiteSpace(fileFormat))
    errors.Add("FileFormat");

if (errors.Count > 0)
    throw new Exception($"Конфигурационный файл не настроен.\nНе заданы параметры:\n {string.Join("\n", errors)}");

var appSettings = new AppSettings(false, false, string.Empty,
    fileFormat, paidBillEventCount, cancelledBillEventCount, 0, string.Empty);

var runner = new AppRunner(appSettings);
runner.Generate();

Console.WriteLine("Генерация завершена.");
Console.ReadLine();