using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace Request.RequestCreate
{
    public class TimeTableCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public Guid? semesterId { get; set; }
        public Guid? gradeId { get; set; }
        public double startDay { get; set; }

    }


    #region code a Hung
    //public class TimeTableCreate : DomainCreate
    //{
    //    public Guid classId { get; set; }
    //    public Guid teacherId { get; set; }
    //    public DateTime date { get; set; }
    //    public Guid studyShiftId { get; set; }
    //    public Guid? roomId { get; set; }//null thì k có phòng học mà học online hoặc học chỗ khác
    //    public string note { get; set; }
    //    public Guid? subjectId { get; set; }
    //    public int status { get; set; }
    //}

    //public class GenerateTimeTableCreate : DomainCreate
    //{
    //    public double startTime { get; set; }
    //    public double endTime { get; set; }

    //    public List<Schedule> schedules { get; set; }
    //}

    //public class Schedule
    //{
    //    public Guid teacherId { get; set; }
    //    public Guid studyShiftId { get; set; }
    //    public Guid? roomId { get; set; }//null thì k có phòng học mà học online hoặc học chỗ khác
    //    public string note { get; set; }
    //    public Guid? subjectId { get; set; }
    //    public int keyDayOfWeek { get; set; }
    //}
    #endregion
}
