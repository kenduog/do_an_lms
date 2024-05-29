using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class PaymentMethodSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? paymentBankId { get; set; }
        public int? type { get; set; }
    }
}
