using dnaborshchikova_github.Bea.Generator.Interfaces;
using dnaborshchikova_github.Bea.Generator.Services;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var paidBillEventCount = int.Parse(config["PaidBillEventCount"]);
var cancelledBillEventCount = int.Parse(config["CancelledBillEventCount"]);
var fileFormat = config["FileFormat"];

IGenerateService generateService = fileFormat switch
{
    "csv" => new CsvGenerateService(),
    "xml" => new XmlGenerateService(),
    _ => new CsvGenerateService()
};

generateService.GenerateFile(paidBillEventCount, cancelledBillEventCount);
Console.ReadLine();