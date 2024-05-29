using System;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities;

namespace Entities
{
    public class tbl_TimeTableDetail : DomainEntities.DomainEntities
    {
        [NotMapped]
        public Guid? semesterId { get; set; }
        [NotMapped]
        public Guid? gradeId { get; set; }
        [NotMapped]
        public Guid? schoolYearId { get; set; }
        [NotMapped]
        public Guid? branchId { get; set; }


        public Guid? timeTableId { get; set; }
        [NotMapped]
        public string className { get; set; }
        public Guid? classId { get; set; }
        [NotMapped]
        public string subjectName { get; set; }
        public Guid? subjectId { get; set; }
        [NotMapped]
        public string teacherName { get; set; }
        public Guid? teacherId { get; set; }
        [NotMapped]
        public string roomName { get; set; }

        /// <summary>
        /// ngay hoc
        /// </summary>
        public int? day { get; set; }
        /// <summary>
        /// tiet hoc
        /// </summary>
        public int? index { get; set; }

        public double sTime { get; set; }
        public double eTime { get; set; }

        public Guid? roomId { get; set; } //null thì k có phòng học mà học online hoặc học chỗ khác
        public string note { get; set; }
        public int status { get; set; } //0: Sắp diễn ra, 1 đã diễn ra
    }

    public class TimeTableDetailResponse
    {
        public Guid id { get; set; }
        public string className { get; set; }
        public Guid? classId { get; set; }
        public string subjectName { get; set; }
        public Guid? subjectId { get; set; }
        public string teacherName { get; set; }
        public Guid? teacherId { get; set; }
        public string roomName { get; set; }
        /// <summary>
        /// ngay hoc
        /// </summary>
        public int? day { get; set; }
        /// <summary>
        /// tiet hoc
        /// </summary>
        public int? index { get; set; }
        public double sTime { get; set; }
        public double eTime { get; set; }
        public Guid? roomId { get; set; } //null thì k có phòng học mà học online hoặc học chỗ khác
        public string note { get; set; }
        public int status { get; set; } //0: Sắp diễn ra, 1 đã diễn ra
        public int? subjectType { get; set; }
    }
}
