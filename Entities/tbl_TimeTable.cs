using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities;

namespace Entities
{
    public class tbl_TimeTable : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? semesterId { get; set; }
        [NotMapped]
        public string semesterName { get; set; }
        public Guid? gradeId { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        public double startDay { get; set; }
    }

    public class TimeTableResponse
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public bool? active { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? semesterId { get; set; }
        public string semesterName { get; set; }
        public Guid? gradeId { get; set; }
        public string gradeName { get; set; }
        public double startDay { get; set; }
        public List<TimeTableItem> schedules { get; set; }
    }
    public class TimeTableItem
    {
        public Guid? id { get; set; }
        public int? day { get; set; }
        public int? index { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public Dictionary<Guid, TimeTableSubItem> items { get; set; }
    }

    public class TimeTableSubItem
    {
        public Guid id { get; set; }
        public Guid? roomId { get; set; }
        public Guid? teacherId { get; set; }
        public Guid? subjectId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        public string className { get; set; }
        public string subjectName { get; set; }
        public string teacherName { get; set; }
        public string roomName { get; set; }
        public int? subjectType { get; set; }
    }
}
