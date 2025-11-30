using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var isRunAsProcess = bool.Parse(config["RunAsProcess"]);

if (isRunAsProcess)
{
    var proccess = new Process();
    proccess.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "dnaborshchikova_github.Bea.Generator.exe"); ;
    proccess.Start();
    return;
}

var fileFormat = config["FileFormat"];
var paidBillEventCount = int.Parse(config["PaidBillEventCount"]);
var cancelledBillEventCount = int.Parse(config["CancelledBillEventCount"]);

var runner = new AppRunner();
runner.Generate(fileFormat, paidBillEventCount, cancelledBillEventCount);

Console.ReadLine();