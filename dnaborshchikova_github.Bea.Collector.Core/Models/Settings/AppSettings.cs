namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class AppSettings
    {
        public ProcessingSettings ProcessingSettings { get; }
        public GeneratorSettings GeneratorSettings { get; }

        public AppSettings(ProcessingSettings processingSettings, GeneratorSettings generatorSettings)
        {
            this.ProcessingSettings = processingSettings
                ?? throw new ArgumentNullException(nameof(processingSettings));
            this.GeneratorSettings = generatorSettings
                ?? throw new ArgumentNullException(nameof(generatorSettings));

            generatorSettings.Validate();
            processingSettings.Validate();
        }
    }
}
