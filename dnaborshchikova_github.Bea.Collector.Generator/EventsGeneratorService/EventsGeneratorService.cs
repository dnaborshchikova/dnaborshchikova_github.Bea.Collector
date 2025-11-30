using dnaborshchikova_github.Bea.Generator.DataGeneration;
using dnaborshchikova_github.Bea.Generator.FileGeneration;

namespace dnaborshchikova_github.Bea.Generator.EventsGeneratorService
{
    public class EventsGeneratorService : IEventsGeneratorService
    {
        private readonly IDataGenerator _dataGenerator;
        private readonly Func<string, IFileGenerator> _fileGeneratorFactory;

        public EventsGeneratorService(IDataGenerator dataGenerator, Func<string, IFileGenerator> fileGeneratorFactory)
        {
            _dataGenerator = dataGenerator;
            _fileGeneratorFactory = fileGeneratorFactory;
        }

        public void GenerateEvents(string fileFormat, int paidBillEventRecordCount
            , int cancelledBillEventRecordCount)
        {
            var events = _dataGenerator.GenerateEvents(paidBillEventRecordCount, cancelledBillEventRecordCount);
            var fileGenerator = _fileGeneratorFactory(fileFormat);
            fileGenerator.GenerateFile(events);
        }
    }
}
