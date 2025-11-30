using Bogus;
using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Generator.DataGeneration
{
    public class DataGenerator : IDataGenerator
    {
        public List<BillEvent> GenerateEvents(int paidBillEventRecordCount, int cancelledBillEventRecordCount)
        {
            var paidBillFaker = new Faker<PaidBillEvent>("ru")
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.OperationDateTime, f => f.Date.Recent(5))
                .RuleFor(x => x.UserId, f => Guid.NewGuid())
                .RuleFor(x => x.EventType, f => "bill_payed")
                .RuleFor(x => x.Amount, f => f.Random.Decimal(100, 5000))
                .RuleFor(x => x.Number, f => f.Random.Int(100000, 999999).ToString())
                .RuleFor(x => x.BuyerId, f => Guid.NewGuid());

            var cancelledBillFaker = new Faker<CancelledBillEvent>("ru")
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.OperationDateTime, f => f.Date.Recent(5))
                .RuleFor(x => x.UserId, f => Guid.NewGuid())
                .RuleFor(x => x.EventType, f => "bill_canceled")
                .RuleFor(x => x.Amount, f => f.Random.Decimal(100, 5000))
                .RuleFor(x => x.CancelAmount, (f, b) => f.Random.Decimal(10, b.Amount))
                .RuleFor(x => x.Number, f => f.Random.Int(100000, 999999).ToString());

            var paidBills = paidBillFaker.Generate(paidBillEventRecordCount);
            var cancelledBills = cancelledBillFaker.Generate(cancelledBillEventRecordCount);
            var allBills = paidBills.Cast<BillEvent>()
                .Concat(cancelledBills)
                .OrderBy(x => x.OperationDateTime)
                .ToList();

            return allBills;
        }

        public void GeneratePayed()
        {
            var paidBillFaker = new Faker<PaidBillEvent>("ru")
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.OperationDateTime, f => f.Date.Recent(5))
                .RuleFor(x => x.UserId, f => Guid.NewGuid())
                .RuleFor(x => x.EventType, f => "bill_payed")
                .RuleFor(x => x.Amount, f => f.Random.Decimal(100, 5000))
                .RuleFor(x => x.Number, f => f.Random.Int(100000, 999999).ToString())
                .RuleFor(x => x.BuyerId, f => f.Random.Bool(0.6f) ? Guid.NewGuid() : (Guid?)null);
        }

        public void GenerateCancelled()
        {
            var cancelledBillFaker = new Faker<CancelledBillEvent>("ru")
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.OperationDateTime, f => f.Date.Recent(5))
                .RuleFor(x => x.UserId, f => Guid.NewGuid())
                .RuleFor(x => x.EventType, f => "bill_canceled")
                .RuleFor(x => x.Amount, f => f.Random.Decimal(100, 5000))
                .RuleFor(x => x.CancelAmount, (f, b) => f.Random.Decimal(10, b.Amount))
                .RuleFor(x => x.Number, f => f.Random.Int(100000, 999999).ToString());
        }
    }
}
