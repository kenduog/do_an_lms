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
    public class SubjectCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_subjectGroupId)]
        public Guid? subjectGroupId { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        [Required(ErrorMessage = MessageContants.req_acronym)]
        public string acronym { get; set; }
        [Required(ErrorMessage = MessageContants.req_remarkType)]
        public int? remarkType { get; set; }
        public int? totalSemester1 { get; set; }
        public int? totalSemester2 { get; set; }
        /// <summary>
        /// 1 = Môn học bắt buộc
        /// 2 = Hoạt động thể chất bắt buộc
        /// 3 = Môn học tự chọn
        /// </summary>
        public int? type { get; set; }
    }
}
