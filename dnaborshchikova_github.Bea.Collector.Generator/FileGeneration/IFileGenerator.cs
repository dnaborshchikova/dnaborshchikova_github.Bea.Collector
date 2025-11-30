using dnaborshchikova_github.Bea.Collector.Core.Models;

namespace dnaborshchikova_github.Bea.Generator.FileGeneration
{
    public interface IFileGenerator
    {
        public void GenerateFile(List<BillEvent> billEvents);
    }
}
