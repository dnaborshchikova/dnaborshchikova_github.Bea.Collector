using Bogus;
using dnaborshchikova_github.Bea.Collector.Generator.Models;
using dnaborshchikova_github.Bea.Generator.Interfaces;

namespace dnaborshchikova_github.Bea.Generator.Services
{
    public class BillEventService : IBillEventService
    {
        public List<BillEventFileRecord> GetBillEvents(int paidBillEventRecordCount
            , int cancelledBillEventRecordCount)
        {
            var date = DateTime.Today;

            var payedFaker = new Faker<BillEventFileRecord>("ru")
                .RuleFor(e => e.OperationDateTime, f => f.Date.Between(date.AddDays(-7), date.AddDays(7)))
                .RuleFor(e => e.UserId, f => f.Random.Guid())
                .RuleFor(e => e.EventType, "bill_payed")
                .RuleFor(e => e.Amount, f => Math.Round(f.Finance.Amount(10, 5000), 2))
                .RuleFor(e => e.CancelAmount, (f, e) => null)
                .RuleFor(e => e.BillNumber, (f, e) => (100000 + f.IndexFaker).ToString())
                .RuleFor(e => e.Id, f => f.Random.Guid())
                .RuleFor(e => e.BuyerId, f => f.Random.Guid());

            var canceledFaker = new Faker<BillEventFileRecord>("ru")
                .RuleFor(e => e.OperationDateTime, f => f.Date.Between(date.AddDays(-7), date.AddDays(7)))
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
