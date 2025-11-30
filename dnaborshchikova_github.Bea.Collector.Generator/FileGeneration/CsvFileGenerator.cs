using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Globalization;
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
                var billEventProperties = typeof(BillEvent).GetProperties();
                var paidBillEventProperties = typeof(PaidBillEvent).GetProperties();
                var cancelledBillEventProperties = typeof(CancelledBillEvent).GetProperties();
                var allProperties = billEventProperties
                    .Concat(paidBillEventProperties)
                    .Concat(cancelledBillEventProperties)
                    .Select(p => p.Name)
                    .Distinct();

                writer.WriteLine(string.Join(",", allProperties));

                foreach (var bill in billEvents)
                {
                    var buyerId = bill is PaidBillEvent p ? p.BuyerId?.ToString() ?? "" : "";
                    var cancelAmount = bill is CancelledBillEvent c ? c.CancelAmount?.ToString(CultureInfo.InvariantCulture) ?? "" : "";

                    writer.WriteLine($"{bill.Id},{bill.OperationDateTime},{bill.UserId},{bill.EventType}" +
                        $",{bill.Amount.ToString(CultureInfo.InvariantCulture)},{bill.Number},{buyerId},{cancelAmount}");
                }
            }
        }
    }
}
