using dnaborshchikova_github.Bea.Generator.Interfaces;
using dnaborshchikova_github.Bea.Generator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var errors = new List<string>();

if (!int.TryParse(config["PaidBillEventCount"], out var paidBillEventCount))
    errors.Add("В конфигурационном файле не задан параметр PaidBillEventCount.");

if (!int.TryParse(config["CancelledBillEventCount"], out var cancelledBillEventCount))
    errors.Add("В конфигурационном файле не задан параметр CancelledBillEventCount.");

var fileFormat = config["FileFormat"];
if (string.IsNullOrWhiteSpace(fileFormat))
    errors.Add("В конфигурационном файле не задан параметр FileFormat.");

if (errors.Count > 0)
    throw new Exception($"Конфигурационный файл не настроен.\nСписок ошибок:\n {string.Join("\n", errors)}");

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<CsvGenerateService>();
        services.AddScoped<XmlGenerateService>();
        services.AddScoped<IBillEventService, BillEventService>();
        services.AddScoped<Func<string, IGenerateService>>(provider => key =>
        {
            return key switch
            {
                "csv" => provider.GetRequiredService<CsvGenerateService>(),
                "xml" => provider.GetRequiredService<XmlGenerateService>(),
                _ => provider.GetRequiredService<CsvGenerateService>()
            };
        });
    })
    .Build();

using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;

var generatorFactory = provider.GetRequiredService<Func<string, IGenerateService>>();
var generator = generatorFactory(fileFormat);

generator.GenerateFile(paidBillEventCount, cancelledBillEventCount);
Console.ReadLine();