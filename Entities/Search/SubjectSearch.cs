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
    public class SubjectSearch : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? subjectGroupId { get; set; }
        public int? type { get; set; }
        public int? remarkType { get; set; }
    }

    public class SubjectPrepareSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? gradeId { get; set; }
    }
}
