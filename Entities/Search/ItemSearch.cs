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
    public class ItemSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? unitOfMeasureId { get; set; }
        public Guid? itemGroupId { get; set; }
    }
}
