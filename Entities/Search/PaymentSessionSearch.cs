using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class PaymentSessionSearch : BaseSearch
    {
        public Guid? billId { get; set; }
        public Guid? branchId { get; set; }
        public int? type { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public int? status { get; set; }
    }
}
