using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Generator.Interfaces
{
    public interface IGenerateService
    {
        public void GenerateFile(int paidBillEventRecordCount, int cancelledBillEventRecordCount);
    }
}
