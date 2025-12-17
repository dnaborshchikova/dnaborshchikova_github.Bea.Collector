namespace dnaborshchikova_github.Bea.Collector.Core.Models.Settings
{
    public class GeneratorSettings
    {
        public string FileFormat { get; set; }
        public int PaidBillEventCount { get; set; }
        public int CancelledBillEventCount { get; set; }

        public GeneratorSettings()
        {

        }

        public GeneratorSettings(string fileFormat, int paidBillEventCount, int cancelledBillEventCount)
        {
            FileFormat = fileFormat;
            PaidBillEventCount = paidBillEventCount;
            CancelledBillEventCount = cancelledBillEventCount;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(FileFormat))
                throw new InvalidOperationException("Не заполнен FileFormat.");

            if (PaidBillEventCount <= 0)
                throw new InvalidOperationException("Не указано PaidBillEventCount");

            if (CancelledBillEventCount <= 0)
                throw new InvalidOperationException("DelayMs не может быть отрицательным");
        }
    }
}
