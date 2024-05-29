using System;

namespace Entities.DataTransferObject
{
    public class GenerateTimeTableDTO
    {
        public Guid teacherId { get; set; }
        public double startTime { get; set; }
        public double endTime { get; set; }
        public Guid? roomId { get; set; }//null thì k có phòng học mà học online hoặc học chỗ khác
        public string note { get; set; }
        public Guid? subjectId { get; set; }
        public bool isAvailable { get; set; }//true là phù hợp, false là không, cần phải sửa lịch
        public string reason { get; set; }//Nguyên nhân không phù hợp
    }
}
