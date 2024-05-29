using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_Week : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Tên tuần
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Năm học
        /// </summary>
        public Guid? schoolYearId { get; set; }
        /// <summary>
        /// Học kỳ
        /// </summary>
        public Guid? semesterId { get; set; }
        /// <summary>
        /// Bắt đầu
        /// </summary>
        public double? sTime { get; set; }
        /// <summary>
        /// Kết thúc
        /// </summary>
        public double? eTime { get; set; }
        /// <summary>
        /// Số tuần
        /// </summary>
        public int? weekNumber { get; set; }
        [NotMapped]
        public string schoolYearName { get; set; }
        [NotMapped]
        public string semesterName { get; set; }
        [NotMapped]
        public string startWeekName
        {
            get
            {
                if (this.sTime == null)
                    return null;
                DateTime sSemesterTime = DateTimeOffset.FromUnixTimeMilliseconds((long)this.sTime.Value).UtcDateTime.ToLocalTime();
                if (sSemesterTime.DayOfWeek > 0)
                    return "Thứ " + ((int)sSemesterTime.DayOfWeek + 1);
                else return "Chủ nhật";
            }
        }
        [NotMapped]
        public string endWeekName
        {
            get
            {
                if (this.eTime == null)
                    return null;
                DateTime eSemesterTime = DateTimeOffset.FromUnixTimeMilliseconds((long)this.eTime.Value).UtcDateTime.ToLocalTime();
                if (eSemesterTime.DayOfWeek > 0)
                    return "Thứ " + ((int)eSemesterTime.DayOfWeek + 1);
                else return "Chủ nhật";
            }
        }
    }
}
