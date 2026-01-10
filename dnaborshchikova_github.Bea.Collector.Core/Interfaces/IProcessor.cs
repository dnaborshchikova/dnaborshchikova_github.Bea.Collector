using dnaborshchikova_github.Bea.Collector.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IProcessor
    {
        public Task ProcessAsync(List<EventProcessRange> ranges);
    }
}
