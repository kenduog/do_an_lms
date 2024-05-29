using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class ReceiveOrderHeaderSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? vendor { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public string typeCode { get; set; }
        public Guid? itemId { get; set; }
    }
}
