using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class ValueEntrySearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? vendorId { get; set; }
        public Guid? itemId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public bool? isDetail { get; set; } = false;
    }
}
