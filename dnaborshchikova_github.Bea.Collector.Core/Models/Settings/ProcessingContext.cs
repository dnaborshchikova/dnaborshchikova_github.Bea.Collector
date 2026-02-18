namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class ProcessingContext
    {
        public string FileName { get; }
        public DateTime RunDateTime { get; }
        public string RunSettings { get; }

        public ProcessingContext(string fileName, DateTime runDateTime, string runSettings)
        {
            this.FileName = fileName;
            this.RunDateTime = runDateTime;
            this.RunSettings = runSettings;
        }
    }
}
