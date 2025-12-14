namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class AppSettings
    {
        public bool RunAsProcess { get; set; }
        public bool GenerateFile { get; set; }
        public string FilePath { get; set; }
        public string FileFormat { get; set; }
        public int PaidBillEventCount { get; set; }
        public int CancelledBillEventCount { get; set; }
        public int ThreadCount { get; set; }
        public string ProcessType { get; set; }

        public AppSettings()
        {

        }

        public AppSettings(bool runAsProcess, bool generateFile, string filePath, string fileFormat
            , int paidBillEventCount, int cancelledBillEventCount, int threadCount, string processType)
        {
            this.RunAsProcess = runAsProcess;
            this.GenerateFile = generateFile;
            this.FilePath = filePath;
            this.FileFormat = fileFormat;
            this.PaidBillEventCount = paidBillEventCount;
            this.CancelledBillEventCount = cancelledBillEventCount;
            this.ThreadCount = threadCount;
            this.ProcessType = processType;
        }
    }
}
