using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Reflection;
using System.Text;

namespace dnaborshchikova_github.Bea.Generator.FileGeneration
{
    public class CsvFileGenerator : IFileGenerator
    {
        public void GenerateFile(List<BillEvent> billEvents)
        {
            var fileName = $"{DateTime.Now.ToShortDateString()}_BillEvent.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var bill in billEvents)
                {

                    var buyerId = bill is PaidBillEvent p ? p.BuyerId?.ToString() ?? "" : "";
                    var cancelAmount = bill is CancelledBillEvent c ? c.CancelAmount?.ToString() ?? "" : "";

                    writer.WriteLine($"{bill.Id},{bill.UserId},{buyerId},{bill.Number},{bill.EventType}" +
                        $",{bill.OperationDateTime:O},{bill.Amount},{cancelAmount}");
                }
            }
        }
    }
}
