using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class ChildAssessmentTopicSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? gradeId { get; set; }
    }
}
