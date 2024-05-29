using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Lưu từ ngày 1/1/2000
    /// </summary>
    public class tbl_StudyShift: DomainEntities.DomainEntities
    {
        public Guid? gradeId { get; set; }
        public Guid? classId { get; set; }
        /// <summary>
        /// Số thứ tự, tương ứng tiết 1, 2, 3... trong buổi
        /// </summary>
        public int? position { get; set; }
        /// <summary>
        /// Thứ trong tuần (1 -> 7) ~ (chủ nhật -> thứ 7)
        /// </summary>
        public int? dayInWeek { get; set; }
        /// <summary>
        /// Buổi học
        /// 1 - Sáng 
        /// 2 - Chiều
        /// 3 - Tối
        /// </summary>
        public int? type { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
