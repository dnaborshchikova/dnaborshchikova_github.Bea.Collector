namespace dnaborshchikova_github.Bea.Generator.EventsGeneratorService
{
    public interface IEventsGeneratorService
    {
        public void GenerateEvents(string fileFormat, int paidBillEventRecordCount
             , int cancelledBillEventRecordCount);
    }
}
