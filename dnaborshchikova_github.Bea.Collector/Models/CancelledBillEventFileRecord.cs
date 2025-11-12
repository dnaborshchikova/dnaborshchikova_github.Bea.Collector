using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.CollertorService.Models
{
    public class CancelledBillEventFileRecord : BillEventFileRecord
    {
        public decimal? CancelAmount { get; set; }
    }
}
