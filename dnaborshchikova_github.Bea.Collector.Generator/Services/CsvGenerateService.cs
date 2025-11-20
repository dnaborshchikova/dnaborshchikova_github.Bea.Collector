using dnaborshchikova_github.Bea.Generator.Interfaces;
using System.Text;

namespace dnaborshchikova_github.Bea.Generator.Services
{
    public class CsvGenerateService : IGenerateService
    {
        public void GenerateFile(int paidBillEventRecordCount, int cancelledBillEventRecordCount)
        {
            var billEvents = BillEventService.GetBillEvents(paidBillEventRecordCount, cancelledBillEventRecordCount);

            var fileName = $"{DateTime.Now.ToShortDateString()}_BillEvent.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("Id,UserId,BuyerId,BillNumber,EventType,OperationDateTime,Amount,CancelAmount");
                foreach (var billEvent in billEvents)
                {
                    writer.WriteLine($"{billEvent.Id},{billEvent.UserId},{billEvent.BuyerId}," +
                    $"{billEvent.BillNumber},{billEvent.EventType},{billEvent.OperationDateTime}," +
                    $"{billEvent.Amount},{billEvent.CancelAmount}");
                }
            }
        }

    }
}
