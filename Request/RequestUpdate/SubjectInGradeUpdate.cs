using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class SubjectInGradeUpdate : DomainUpdate
    {
        public int? duration  { get; set; }
        /// <summary>
        /// SỐ tiết học kỳ 1
        /// </summary>
        public int? lessonInSemester1 { get; set; }
        /// <summary>
        /// Số tiết học kỳ 2
        /// </summary>
        public int? lessonInSemester2 { get; set; }
    }
}
