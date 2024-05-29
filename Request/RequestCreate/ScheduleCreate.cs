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
    public class ScheduleCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_timeTableId)]
        public Guid? timeTableId { get; set; }
        [Required(ErrorMessage = MessageContants.req_weekId)]
        public Guid? weekId { get; set; }
        [Required(ErrorMessage = MessageContants.req_subjectId)]
        public Guid? subjectId { get; set; }
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_teacherId)]
        public Guid? teacherId { get; set; }
        public double? sTime { get; set; }
        [LargerStartTime]
        public double? eTime { get; set; }
        /// <summary>
        /// 1 - Tiết trong tkb
        /// 2 - Tiết học bù
        /// </summary>
        public int? type { get; set; } = 2;
    }

    public class ScheduleNotificationRequest
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationTitle)]
        public string title { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationContent)]
        public string content { get; set; }
    }
}
