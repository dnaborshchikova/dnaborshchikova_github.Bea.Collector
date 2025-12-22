namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class ProcessingSettings
    {
        public bool RunAsProcess { get; set; }
        public bool GenerateFile { get; set; }
        public string FilePath { get; set; }
        public int ThreadCount { get; set; }
        public string ProcessType { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("Не указан путь к файлу");

            if (ThreadCount <= 0 )
                throw new InvalidOperationException($"Не указано количество потоков.");

            if (string.IsNullOrEmpty(ProcessType))
                throw new InvalidOperationException($"Не указан тип обработки.");
        }
    }
}
