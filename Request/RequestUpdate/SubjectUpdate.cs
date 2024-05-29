using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class SubjectUpdate : DomainUpdate
    {
        public Guid? subjectGroupId { get; set; }
        public string name { get; set; }
        public string acronym { get; set; }
        public int? remarkType { get; set; }
        public int? totalSemester1 { get; set; }
        public int? totalSemester2 { get; set; }
        /// <summary>
        /// 1 = Môn học bắt buộc
        /// 2 = Hoạt động thể chất bắt buộc
        /// 3 = Môn học tăng cường
        /// 4 = Môn học tự chọn
        /// </summary>
        public int? type { get; set; }
    }
}
