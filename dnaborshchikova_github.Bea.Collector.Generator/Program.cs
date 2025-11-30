using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Генератор запущен.");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

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

Console.WriteLine("Генерация завершена.");
Console.ReadLine();