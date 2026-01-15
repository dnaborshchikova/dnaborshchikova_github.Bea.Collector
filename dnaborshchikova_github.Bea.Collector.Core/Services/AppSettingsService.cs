using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.Core.Services
{
    public class AppSettingsService
    {
        public AppSettings CreateAppSettings(GeneratorSettings generatorSettings
            , ProcessingSettings processingSettings)
        {
            if (generatorSettings == null)
                throw new Exception("Не заполнена секция GeneratorSettings в конфигурационном файле.");
            if (processingSettings == null)
                throw new Exception("Не заполнена секция ProcessingSettings в конфигурационном файле.");

            return new AppSettings(processingSettings, generatorSettings);
        }
    }
}
