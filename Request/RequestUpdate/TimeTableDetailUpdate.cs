using Request.DomainRequests;
using Request.RequestCreate;
using System;
using System.Collections.Generic;

namespace Request.RequestUpdate
{
    public class TimeTableDetailUpdate : DomainUpdate
    {
        public Guid? subjectId { get; set; }
        public Guid? teacherId { get; set; }
        public int? index { get; set; }
        public double sTime { get; set; }
        public double eTime { get; set; }
        public string note { get; set; }

        public Guid? roomId { get; set; } //null thì k có phòng học mà học online hoặc học chỗ khác
        public int status { get; set; } //0: Sắp diễn ra, 1 đã diễn ra
    }
}
