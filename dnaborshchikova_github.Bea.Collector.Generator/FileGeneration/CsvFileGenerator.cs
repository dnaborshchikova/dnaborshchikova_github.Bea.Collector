using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Globalization;
using System.Text;

namespace dnaborshchikova_github.Bea.Generator.FileGeneration
{
    public class CsvFileGenerator : IFileGenerator
    {
        public void GenerateFile(List<BillEvent> billEvents)
        {
            var fileName = $"{DateTime.Now.ToShortDateString()}_BillEvent.csv";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var billEventProperties = typeof(BillEvent).GetProperties();
                var paidBillEventProperties = typeof(PaidBillEvent).GetProperties();
                var cancelledBillEventProperties = typeof(CancelledBillEvent).GetProperties();
                var allProperties = billEventProperties
                    .Concat(paidBillEventProperties)
                    .Concat(cancelledBillEventProperties)
                    .GroupBy(p => p.Name)
                    .Select(g => g.First())
                    .ToList();

                writer.WriteLine(string.Join(",", allProperties.Select(p => p.Name)));

                foreach (var bill in billEvents)
                {
                    var billRecord = allProperties.Select(p =>
                    {
                        var property = bill.GetType().GetProperty(p.Name);
                        if (property == null)
                            return string.Empty;

                        var value = p.GetValue(bill);

                        if (value == null)
                            return string.Empty;

                        if (value is decimal decimalValue)
                            return decimalValue.ToString(CultureInfo.InvariantCulture);

                        return value.ToString();
                    });

                    writer.WriteLine(string.Join(",", billRecord));
                }
            }
        }
    }
}
