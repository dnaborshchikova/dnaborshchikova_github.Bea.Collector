using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using Microsoft.Extensions.Configuration;

namespace dnaborshchikova_github.Bea.Collector.App
{
    public class AppSettingsService
    {
        private readonly IConfiguration _configuration;

        public AppSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AppSettings CreateAppSettings()
        {
            var generatorSettings = _configuration
                .GetSection(nameof(GeneratorSettings))
                .Get<GeneratorSettings>() 
                ?? throw new Exception("Не заполнена секция GeneratorSettings в конфигурационном файле.");
            var processingSettings = _configuration
                .GetSection(nameof(ProcessingSettings))
                .Get<ProcessingSettings>() 
                ?? throw new Exception("Не заполнена секция ProcessingSettings в конфигурационном файле.");

            var appSettings = new AppSettings(processingSettings, generatorSettings);
            return appSettings;
        }
    }
}
