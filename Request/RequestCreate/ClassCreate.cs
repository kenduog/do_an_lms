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
    public class ClassCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public int? size { get; set; }
        [Required (ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        [Required (ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        public Guid? teacherId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        public Guid? branchId { get; set; }
        public List<ClassShiftCreate> shifts { get; set; }

    }

    public class ClassShiftCreate
    {
        public Guid? id { get; set; }
        [Required(ErrorMessage = MessageContants.req_day)]
        public int? day { get; set; }
        /// <summary>
        /// tiết
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_period)]
        public int? period { get; set; }
        /// <summary>
        /// Giờ bắt đầu, tính tại 01/01/1970 GMT
        /// </summary>
        public double? sTime { get; set; }
        /// <summary>
        /// Giờ kết thúc, tính tại 01/01/1970 GMT
        /// </summary>
        [LargerStartTime]
        public double? eTime { get; set; }
    }

    /// <summary>
    /// Tạo nhiều lớp cùng lúc
    /// </summary>
    public class MultipleClassCreate : DomainCreate
    {
        public List<ClassItem> classes { get; set; }
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
    }

    public class ClassItem
    {
        [Required(ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }

        public int? totalClass { get; set; }

        /// <summary>
        /// Chữ cái bắt đầu của lớp: lấy từ tbl_Grade
        /// </summary>
        public string startWith { get; set; }

        /// <summary>
        /// 1 - Tăng theo ABC
        /// 2 - Tăng theo /123
        /// </summary>
        public int? increaseWith { get; set; }

        /// <summary>
        /// Chữ cái đầu tiên
        /// </summary>
        public string firstLetter { get; set; }
        /// <summary>
        /// Số lượng học sinh mỗi lớp - Mặc định 20
        /// </summary>
        public int? size { get; set; } = 20;
    }
}
