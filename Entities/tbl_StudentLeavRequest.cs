using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_StudentLeaveRequest : DomainEntities.DomainEntities
    {
        public Guid? classId { get; set; }
        public Guid? studentId { get; set; }
        public double? fromDate { get; set; }
        public double? toDate { get; set; }
        public string description { get; set; }
    }
}
