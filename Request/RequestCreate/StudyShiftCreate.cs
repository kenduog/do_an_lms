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
    public class StudyShiftCreate : DomainCreate
    {
        /// <summary>
        /// Chi nhánh (trường) áp dụng
        /// </summary>
        public Guid? branchId { get; set; }

        /// <summary>
        /// Những khối được áp dụng (bỏ tróng là áp dụng cho toàn trường (chi nhánh))
        /// </summary>
        public List<Guid> gradeIds { get; set; }

        /// <summary>
        /// Những lớp được áp dụng (bỏ trống là áp dụng cho tất cả các lớp trong khối được chọn)
        /// </summary>
        public List<Guid> classIds { get; set; }
        public List<ItemStudyShift> items { get; set; }

    }

    public class ItemStudyShift
    {

        [Required(ErrorMessage = MessageContants.req_stime)]
        public double? sTime { get; set; }

        [Required(ErrorMessage = MessageContants.req_etime)]
        [LargerStartTime]
        public double? eTime { get; set; }

        /// <summary>
        /// Số thứ tự, tương ứng tiết 1, 2, 3... trong buổi
        /// </summary>
        [Range(1, 5, ErrorMessage = "Một buổi chỉ có từ 1 đến 5 tiết học")]
        public int? position { get; set; }

        /// <summary>
        /// Thứ trong tuần (1 -> 7) ~ (chủ nhật -> thứ 7)
        /// </summary>
        [Range(1, 7, ErrorMessage = "Thứ trong tuần (1 -> 7) ~ (chủ nhật -> thứ 7)")]
        public int? dayInWeek { get; set; }

        /// <summary>
        /// Buổi học
        /// 1 - Sáng 
        /// 2 - Chiều
        /// 3 - Tối
        /// </summary>
        [Range(1, 3, ErrorMessage = "Buổi học là sáng(1), chiều(2), tối(3)")]
        public int? type { get; set; }
    }

}
