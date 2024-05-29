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
    public class SubjectInGradeCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public List<ItemSubjectInGrade> items { get; set; }
    }

    public class ItemSubjectInGrade
    {
        [Required(ErrorMessage = MessageContants.req_subjectId)]
        public Guid? subjectId { get; set; }
        /// <summary>
        /// SỐ tiết học kỳ 1
        /// </summary>
        public int? lessonInSemester1 { get; set; }
        /// <summary>
        /// Số tiết học kỳ 2
        /// </summary>
        public int? lessonInSemester2 { get; set; }
        public int? duration { get; set; }
    }
}
