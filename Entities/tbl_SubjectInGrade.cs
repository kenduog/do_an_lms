using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    /// <summary>
    /// Khai báo môn học cho cấp lớp
    /// </summary>
    public class tbl_SubjectInGrade : DomainEntities.DomainEntities
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? subjectId { get; set; }
        public Guid? gradeId { get; set; }
        /// <summary>
        /// Thời gian học
        /// </summary>
        public int? duration { get; set; }
        /// <summary>
        /// SỐ tiết học kỳ 1
        /// </summary>
        public int? lessonInSemester1 { get; set; }
        /// <summary>
        /// Số tiết học kỳ 2
        /// </summary>
        public int? lessonInSemester2 { get; set; }

        [NotMapped]
        public int? totalLesson { get; set; }

        [NotMapped]
        public string gradeName { get; set; }

        [NotMapped]
        public string subjectName { get; set; }
        [NotMapped]
        public int? subjectType { get; set; }
        [NotMapped]
        public int? subjectRemarkType { get; set; }
        [NotMapped]
        public string subjectGroupName{ get; set; }
        [NotMapped]
        public bool isSelected { get; set; } = false;
    }


}
