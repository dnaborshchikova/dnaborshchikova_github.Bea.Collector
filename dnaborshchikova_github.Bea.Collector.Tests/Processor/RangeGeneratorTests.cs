using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor
{
    public class RangeGeneratorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public void GenerateParts_ThreadCount_RangesCountEqualsThreadCount(int threadCount)
        {
            //Arrange
            var userId = Guid.NewGuid();
            var billEvents = new List<BillEvent>
            {
                new(Guid.NewGuid(), new DateTime(2025, 12, 10, 10, 0, 0), userId, "bill_payed", 100m, "INV-001"),
                new(Guid.NewGuid(), new DateTime(2025, 12, 10, 12, 0, 0), userId, "bill_canceled", 200m, "INV-002"),
                new(Guid.NewGuid(), new DateTime(2025, 12, 11, 9, 0, 0), userId, "bill_payed", 150m, "INV-003"),
                new(Guid.NewGuid(), new DateTime(2025, 12, 12, 18, 30, 0), userId, "bill_payed", 300m, "INV-004"),

                new(Guid.NewGuid(), new DateTime(2024, 12, 10, 10, 0, 0), userId, "bill_payed", 100m, "INV-001"),
                new(Guid.NewGuid(), new DateTime(2024, 12, 10, 12, 0, 0), userId, "bill_canceled", 200m, "INV-002"),
                new(Guid.NewGuid(), new DateTime(2025, 07, 11, 9, 0, 0), userId, "bill_payed", 150m, "INV-003"),
                new(Guid.NewGuid(), new DateTime(2025, 08, 12, 18, 30, 0), userId, "bill_payed", 300m, "INV-004"),


                new(Guid.NewGuid(), new DateTime(2025, 01, 10, 10, 0, 0), userId, "bill_payed", 100m, "INV-001"),
                new(Guid.NewGuid(), new DateTime(2025, 01, 10, 12, 0, 0), userId, "bill_canceled", 200m, "INV-002"),
                new(Guid.NewGuid(), new DateTime(2025, 03, 11, 9, 0, 0), userId, "bill_payed", 150m, "INV-003"),
                new(Guid.NewGuid(), new DateTime(2025, 04, 12, 18, 30, 0), userId, "bill_payed", 300m, "INV-004"),

                new(Guid.NewGuid(), new DateTime(2025, 12, 11, 9, 0, 0), userId, "bill_payed", 150m, "INV-003"),
                new(Guid.NewGuid(), new DateTime(2025, 12, 12, 18, 30, 0), userId, "bill_payed", 300m, "INV-004")
            };

            //Act
            var ranges = RangeGenerator.GenerateParts(billEvents, threadCount);

            //Assert
            ranges.Count.Should().Be(threadCount);
            ranges.ForEach(r => r.BillEvents.Count.Should().NotBe(0));
        }
    }
}
