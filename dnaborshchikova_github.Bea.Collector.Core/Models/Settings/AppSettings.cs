namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class AppSettings
    {
        public ProcessingSettings ProcessingSettings { get; }
        public GeneratorSettings GeneratorSettings { get; }

        public AppSettings(ProcessingSettings processingSettings, GeneratorSettings generatorSettings)
        {
            generatorSettings.Validate();
            processingSettings.Validate();

            this.ProcessingSettings = processingSettings;
            this.GeneratorSettings = generatorSettings;
        }
    }
}
