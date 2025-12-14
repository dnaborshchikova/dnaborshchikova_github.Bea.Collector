using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Generator.DataGeneration;
using dnaborshchikova_github.Bea.Generator.FileGeneration;
using System.Diagnostics.CodeAnalysis;

namespace dnaborshchikova_github.Bea.Generator.EventsGeneratorService
{
    public class EventsGeneratorService : IEventsGeneratorService
    {
        private readonly AppSettings _appSettings;
        private readonly IDataGenerator _dataGenerator;
        private readonly Func<string, IFileGenerator> _fileGeneratorFactory;

        public EventsGeneratorService(IDataGenerator dataGenerator, Func<string, IFileGenerator> fileGeneratorFactory
            , AppSettings appSettings)
        {
            _appSettings = appSettings;
            _dataGenerator = dataGenerator;
            _fileGeneratorFactory = fileGeneratorFactory;
        }

        public void GenerateEvents()
        {
            var events = _dataGenerator.GenerateEvents(_appSettings.PaidBillEventCount, _appSettings.CancelledBillEventCount);
            var fileGenerator = _fileGeneratorFactory(_appSettings.FileFormat);
            fileGenerator.GenerateFile(events);
        }
    }
}
