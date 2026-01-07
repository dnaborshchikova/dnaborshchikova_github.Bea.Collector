using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor
{
    public class EventProcessorTests
    {
        #region helpers

        private EventProcessorService CreateSut(Func<string, IProcessor> processor = null
            , IParser parcer = null, AppSettings appSettings = null
            , ILogger<EventProcessorService> logger = null)
        {
            return new EventProcessorService(processor, parcer, appSettings, logger);
        }

        private static AppSettings CreateAppSettings(int threadCount)
        {
            var processingSettings = new ProcessingSettings
            {
                GeneratorRunAsProcess = false,
                GenerateFile = false,
                FilePath = "C:\\13.12.2025_BillEvent.csv",
                ThreadCount = threadCount,
                ProcessType = "Thread"
            };
            var generatorSettings = new GeneratorSettings
            {
                FileFormat = "csv",
                PaidBillEventCount = 700_000,
                CancelledBillEventCount = 300_000
            };

            return new AppSettings(processingSettings, generatorSettings);
        }
        #endregion

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void Process_ValidBillEvents_CallsParserAndProcessorOnce(int threadCount)
        {
            //Arrange
            var appSettings = CreateAppSettings(threadCount);
            var logger = NullLogger<EventProcessorService>.Instance;

            var parserMock = new Mock<IParser>();
            parserMock
                .Setup(p => p.Parse(It.IsAny<string>()))
                .Returns(new List<BillEvent>
                {
                    new BillEvent { OperationDateTime = new DateTime(2024, 1, 2) },
                    new BillEvent { OperationDateTime = new DateTime(2024, 1, 1) }
                });

            var processorMock = new Mock<IProcessor>();
            var processorFactoryMock = new Mock<Func<string, IProcessor>>();
            processorFactoryMock.Setup(f => f("Thread")).Returns(processorMock.Object);

            var service = CreateSut(processorFactoryMock.Object, parserMock.Object, appSettings, logger);
           
            //Act
            service.Process();

            //Assert
            parserMock.Verify(p => p.Parse(appSettings.ProcessingSettings.FilePath), Times.Once);
            processorFactoryMock.Verify(f => f(appSettings.ProcessingSettings.ProcessType), Times.Once);
            processorMock.Verify(p => p.Process(It.IsAny<List<EventProcessRange>>()), Times.Once);
        }
    }
}
