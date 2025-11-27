using dnaborshchikova_github.Bea.Generator.Interfaces;
using System.Globalization;
using System.Text;

namespace dnaborshchikova_github.Bea.Generator.Services
{
    public class CsvGenerateService : IGenerateService
    {
        private readonly IBillEventService _billEventService;

        public CsvGenerateService(IBillEventService billEventService)
        {
            _billEventService = billEventService;
        }

        public void GenerateFile(int paidBillEventRecordCount, int cancelledBillEventRecordCount)
        {
            var billEvents = _billEventService.GetBillEvents(paidBillEventRecordCount, cancelledBillEventRecordCount);

            var fileName = $"{DateTime.Now.ToShortDateString()}_BillEvent.csv";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("Id,UserId,BuyerId,BillNumber,EventType,OperationDateTime,Amount,CancelAmount");
                foreach (var billEvent in billEvents)
                {
                    var amountStr = billEvent.Amount.ToString(CultureInfo.InvariantCulture);
                    var cencelledAmountStr = billEvent.CancelAmount?.ToString(CultureInfo.InvariantCulture);
                    writer.WriteLine($"{billEvent.Id},{billEvent.UserId},{billEvent.BuyerId}," +
                    $"{billEvent.BillNumber},{billEvent.EventType},{billEvent.OperationDateTime}," +
                    $"{amountStr},{cencelledAmountStr}");
                }
            }
        }

    }
}
