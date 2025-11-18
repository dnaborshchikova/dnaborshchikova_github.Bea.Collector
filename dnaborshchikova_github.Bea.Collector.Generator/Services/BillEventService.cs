using Bogus;
using dnaborshchikova_github.Bea.Collector.Generator.Models;

namespace dnaborshchikova_github.Bea.Generator.Services
{
    public static class BillEventService
    {
        public static List<BillEventFileRecord> GetBillEvents(int paidBillEventRecordCount
            , int cancelledBillEventRecordCount)
        {
            var date = DateTime.Today;

            var payedFaker = new Faker<BillEventFileRecord>("ru")
                .RuleFor(e => e.OperationDateTime, f => f.Date.Between(date.AddHours(0), date.AddHours(23.999)))
                .RuleFor(e => e.UserId, f => f.Random.Guid())
                .RuleFor(e => e.EventType, "bill_payed")
                .RuleFor(e => e.Amount, f => Math.Round(f.Finance.Amount(10, 5000), 2))
                .RuleFor(e => e.CancelAmount, (f, e) => null)
                .RuleFor(e => e.BillNumber, (f, e) => (100000 + f.IndexFaker).ToString())
                .RuleFor(e => e.Id, f => f.Random.Guid())
                .RuleFor(e => e.BuyerId, f => f.Random.Bool(0.5f) ? f.Random.Guid() : (Guid?)null);

            var canceledFaker = new Faker<BillEventFileRecord>("ru")
                .RuleFor(e => e.OperationDateTime, f => f.Date.Between(date.AddHours(0), date.AddHours(23.999)))
                .RuleFor(e => e.UserId, f => f.Random.Guid())
                .RuleFor(e => e.EventType, "bill_canceled")
                .RuleFor(e => e.Amount, f => Math.Round(f.Finance.Amount(10, 5000), 2))
                .RuleFor(e => e.CancelAmount, (f, e) => Math.Round(f.Finance.Amount(1, e.Amount), 2))
                .RuleFor(e => e.BillNumber, (f, e) => (100000 + f.IndexFaker).ToString())
                .RuleFor(e => e.Id, f => f.Random.Guid())
                .RuleFor(e => e.BuyerId, (f, e) => null);

            var payed = payedFaker.Generate(paidBillEventRecordCount);
            var canceled = canceledFaker.Generate(cancelledBillEventRecordCount);
            var billEvents = payed.Concat(canceled).OrderBy(e => e.OperationDateTime).ToList();

            return billEvents;
        }
    }
}
