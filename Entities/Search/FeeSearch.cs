using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class FeeSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
    }

    public class FeeOptionSearch 
    {
        public Guid? branchId { get; set; }
        public Guid? gradeId { get; set; }
    }
    public class GetFeeByCollectionPlanRequest : BaseSearch
    {
        public Guid? collectionPlanId { get; set; }
    }
}
