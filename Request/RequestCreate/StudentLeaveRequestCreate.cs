using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class StudentLeaveRequestCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        public double? fromDate { get; set; }
        [LargerFromDateAttribute]
        public double? toDate { get; set; }
        public string description { get; set; }
    }

    public class ItemMultipleLeaveRequest : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        public double? fromDate { get; set; }
        [LargerFromDateAttribute]
        public double? toDate { get; set; }
        public string description { get; set; }
    }

    public class MultipleStudentLeaveRequest
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }

        public List<ItemMultipleLeaveRequest> requests { get; set; }
    }
}
