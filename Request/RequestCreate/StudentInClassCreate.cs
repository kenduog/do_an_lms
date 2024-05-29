using Entities;
using Newtonsoft.Json;
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
    public class StudentInClassCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public Guid? studentId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        public Guid? classId { get; set; }
        /// <summary>
        /// Trạng thái học sinh trong lớp
        /// 1 - Đang học
        /// 2 - Đã tốt nghiệp
        /// 3 - Thôi học
        /// 4 - Bảo lưu
        /// 5 - Học lại
        /// </summary>
        [JsonIgnore]
        public int? status { get { return 1; } }
        [JsonIgnore]
        public string statusName { get { return tbl_StudentInClass.GetStatusName(1); } }
        public string note { get; set; }
    }
    public class MultipleStudentInClassCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        public List<Guid> studentIds { get; set; }
        public string note { get; set; }
    }
}
