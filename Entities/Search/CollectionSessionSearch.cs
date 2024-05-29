using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class CollectionSessionSearch : BaseSearch
    {
        public Guid? collectionPlanId { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }
    }

    public class CollectionSessionDebtSearch : BaseSearch
    {
        public Guid? collectionPlanId { get; set; }
        public Guid? branchId { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }
    }

    public class CollectionSessionByParentSearch : BaseSearch
    {
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }
    }

    public class CollectionSessionByIdSearch : BaseSearch
    {
        public Guid? id { get; set; }
    }


}
