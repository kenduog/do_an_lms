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
    public class ScheduleUpdate : DomainUpdate
    {
        public double? sTime { get; set; }
        [LargerStartTime]
        public double? eTime { get; set; }
        public Guid? subjectId { get; set; }
        public Guid? teacherId { get; set; }
        /// <summary>
        /// 1 - Tiết trong tkb
        /// 2 - Tiết học bù
        /// </summary>
        public int? type { get; set; }
    }
}
