using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class GoodBehaviorCertificateSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? weekId { get; set; }
        public Guid? classId { get; set; }
    }

    public class WeekReportRequest
    {
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        [Required(ErrorMessage = MessageContants.req_weekId)]
        public Guid? weekId { get; set; }
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
    }
}
