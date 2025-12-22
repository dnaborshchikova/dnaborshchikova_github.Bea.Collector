using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Generator
{
    public class GeneratorSettingsService
    {
        private readonly IConfiguration _configuration;

        public GeneratorSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AppSettings GetSettings()
        {
            var settings = _configuration.GetSection(nameof(GeneratorSettings)).Get<GeneratorSettings>();
            settings.Validate();

            return new AppSettings(null, settings);
        }
    }
}
