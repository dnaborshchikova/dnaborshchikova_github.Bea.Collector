using dnaborshchikova_github.Bea.Collector.Generator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Generator.Interfaces
{
    public interface IBillEventService
    {
        public List<BillEventFileRecord> GetBillEvents(int paidBillEventRecordCount
             , int cancelledBillEventRecordCount);
    }
}
