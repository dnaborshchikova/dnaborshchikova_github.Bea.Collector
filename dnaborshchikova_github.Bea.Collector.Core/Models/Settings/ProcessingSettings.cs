namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class ProcessingSettings
    {
        public bool GeneratorRunAsProcess { get; set; }
        public bool GenerateFile { get; set; }
        public string FilePath { get; set; }
        public int ThreadCount { get; set; }
        public string ProcessType { get; set; }
        public string InputFolder { get; set; }

        public void Validate()
        {
            bool invalidRunAsProcess = GeneratorRunAsProcess && !GenerateFile;
            if (invalidRunAsProcess)
            {
                throw new ArgumentException(
                    "GeneratorRunAsProcess требует GenerateFile = true");
            }

            if (!GeneratorRunAsProcess && !GenerateFile && string.IsNullOrEmpty(this.FilePath))
                throw new InvalidOperationException("FilePath обязателен," +
                    "когда генератор не запускается и GenerateFile = false.");

            if (ThreadCount <= 0 )
                throw new InvalidOperationException($"Не указано количество потоков.");

            if (string.IsNullOrEmpty(this.ProcessType))
                throw new InvalidOperationException($"Не указан тип обработки.");
        }
    }
}
