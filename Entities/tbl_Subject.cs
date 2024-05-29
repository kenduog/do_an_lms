using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Subject : DomainEntities.DomainEntities
    {
        public Guid? subjectGroupId { get; set; }
        [NotMapped]
        public string subjectGroupName { get; set; }
        public string name { get; set; }
        public string acronym { get; set; }
        public int? remarkType { get; set; }
        public int? totalSemester1 { get; set; }
        public int? totalSemester2 { get; set; }
        /// <summary>
        /// 1 = Môn học bắt buộc
        /// 2 = Hoạt động thể chất bắt buộc
        /// 3 = Môn học tự chọn
        /// </summary>
        public int? type { get; set; }
        [NotMapped]
        public Guid? schoolYearId { get; set; }
        [NotMapped]
        public Guid? branchId { get; set; }
    }


    public class SubjectPrepare
    {
        public Guid? id { get; set; }
        public string name { get; set; }
        public int? duration { get; set; }
    }
}
