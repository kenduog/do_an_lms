using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class ItemInventorySearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? itemGroupId { get; set; }
    }
}
