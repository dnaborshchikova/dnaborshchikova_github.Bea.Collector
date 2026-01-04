using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;

Console.Error.WriteLine("Генератор запущен.");

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var settingsService = new GeneratorSettingsService(config);
var settings = settingsService.GetSettings();

var runner = new AppRunner(settings);
var filePath = runner.Generate();

Console.Error.WriteLine("Генерация завершена");
Console.Out.WriteLine(filePath);