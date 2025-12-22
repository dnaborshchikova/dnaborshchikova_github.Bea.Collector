using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using System.Globalization;

namespace dnaborshchikova_github.Bea.Collector.Parser.Handlers
{
    public class CsvParser : IParcer
    {
        public List<BillEvent> Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new InvalidOperationException($"Не найден файл по пути {filePath}");

            var cancelledBills = new List<CancelledBillEvent>();
            var paidBills = new List<PaidBillEvent>();

            using (var stream = new StreamReader(filePath))
            {
                var header = stream.ReadLine();

                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var stringParts = line.Split(',');
                    var operationId = new Guid(stringParts[0]);
                    var operationDate = DateTime.Parse(stringParts[1]);
                    var userId = new Guid(stringParts[2]);
                    var eventType = stringParts[3];
                    var amount = Decimal.Parse(stringParts[4], CultureInfo.InvariantCulture);
                    var number = stringParts[5];

                    if (eventType == "bill_payed")
                    {
                        var buyerId = new Guid(stringParts[6]);
                        var payedBillEvent = new PaidBillEvent(operationId, operationDate, userId, eventType, amount, number, buyerId);
                        paidBills.Add(payedBillEvent);
                    }
                    else
                    {
                        var cancellAmount = Decimal.Parse(stringParts[4], CultureInfo.InvariantCulture);
                        var cancelledBillEvent = new CancelledBillEvent(operationId, operationDate, userId, eventType, amount, number, cancellAmount);
                        cancelledBills.Add(cancelledBillEvent);
                    }
                }
            }

            var billEvents = new List<BillEvent>();
            billEvents.AddRange(cancelledBills);
            billEvents.AddRange(paidBills);

            return billEvents;
        }
    }
}
