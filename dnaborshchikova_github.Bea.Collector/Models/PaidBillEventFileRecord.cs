using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.CollertorService.Models
{
    public class PaidBillEventFileRecord : BillEventFileRecord
    {
        public Guid? BuyerId { get; set; }
    }
}
