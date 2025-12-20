using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Генератор запущен.");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settingsService = new GeneratorSettingsService(config);
var settings = settingsService.GetSettings();

var runner = new AppRunner(settings);
runner.Generate();

Console.WriteLine("Генерация завершена.");
Console.ReadLine();