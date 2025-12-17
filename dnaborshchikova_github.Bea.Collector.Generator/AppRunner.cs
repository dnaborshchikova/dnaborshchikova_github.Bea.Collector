using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Generator.DataGeneration;
using dnaborshchikova_github.Bea.Generator.EventsGeneratorService;
using dnaborshchikova_github.Bea.Generator.FileGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dnaborshchikova_github.Bea.Generator
{
    public class AppRunner
    {
        private readonly IHost _host;

        public AppRunner(GeneratorSettings generatorSettings)
        {
            _host = Host.CreateDefaultBuilder()
                     .ConfigureServices(services =>
                     {
                         services.AddSingleton(generatorSettings);
                         services.AddScoped<CsvFileGenerator>();
                         services.AddScoped<XmlFileGenerator>();
                         services.AddScoped<Func<string, IFileGenerator>>(provider => key =>
                         {
                             return key switch
                             {
                                 "csv" => provider.GetRequiredService<CsvFileGenerator>(),
                                 "xml" => provider.GetRequiredService<XmlFileGenerator>(),
                                 _ => provider.GetRequiredService<CsvFileGenerator>()
                             };
                         });
                         services.AddScoped<IDataGenerator, DataGenerator>();
                         services.AddScoped<IEventsGeneratorService, EventsGeneratorService.EventsGeneratorService>();
                     })
                     .Build();
        }

        public void Generate()
        {
            using var scope = _host.Services.CreateScope();
            var eventsGeneratorService = scope.ServiceProvider.GetRequiredService<IEventsGeneratorService>();
            eventsGeneratorService.GenerateEvents();
        }
    }
}
