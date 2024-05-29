using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace Request.RequestCreate
{
    public class TimeTableDetailCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_timeTableId)]
        public Guid? timeTableId { get; set; }
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_subjectId)]
        public Guid? subjectId { get; set; }
        [Required(ErrorMessage = MessageContants.req_teacherId)]
        public Guid? teacherId { get; set; }

        /// <summary>
        /// ngay hoc
        /// </summary>
        public int? day { get; set; }
        /// <summary>
        /// tiet hoc
        /// </summary>
        public int? index { get; set; }

        public double sTime { get; set; }
        [LargerStartTime]
        public double eTime { get; set; }

        public Guid? roomId { get; set; } //null thì k có phòng học mà học online hoặc học chỗ khác
        public string note { get; set; }
        public int status { get; set; } //0: Sắp diễn ra, 1 đã diễn ra
    }
}
