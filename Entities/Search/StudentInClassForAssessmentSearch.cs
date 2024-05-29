using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class StudentInClassForAssessmentSearch : BaseSearch
    {
        public Guid? semesterId { get; set; }
        public Guid? assessmentTopicId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? classId { get; set; }
    }

    public class CriteriaResult : BaseSearch
    {
        public Guid? studentId { get; set; }
        public Guid? semesterId { get; set; }
        public Guid? classId { get; set; }
    }
}
