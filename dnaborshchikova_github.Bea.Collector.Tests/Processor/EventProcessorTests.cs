using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using dnaborshchikova_github.Bea.Collector.Tests.Processor.Builders;
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
        #endregion

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void Process_ValidBillEvents_CallsParserAndProcessorOnce(int threadCount)
        {
            //Arrange
            var generatorSettings = GeneratorSettingsBuilder.Default()
                .Build();

            var processingSettings = ProcessingSettingsBuilder.Default()
                .WithThreadCount(threadCount)
                .WithProcessType("Thread")
                .Build();

            var appSettings = new AppSettingsBuilder()
                .WithGeneratorSettings(generatorSettings)
                .WithProcessingSettings(processingSettings)
                .Build();

            var logger = NullLogger<EventProcessorService>.Instance;
            var parserMock = new Mock<IParser>();
            parserMock.Setup(p => p.Parse(It.IsAny<string>()))
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
