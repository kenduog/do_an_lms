using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class tbl_TeachingAssignment : DomainEntities.DomainEntities
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid teacherId { get; set; }
        public Guid subjectId { get; set; }

        [NotMapped]
        public string teacherIds { get; set; }
        [NotMapped]
        public string teacherNames { get; set; }
        [NotMapped]
        public string subjectName { get; set; }
    }

    public class TeacherBySubjectReponse
    {
        public string teacherName { get; set; }
        public string teacherCode { get; set; }
        public Guid? id { get; set; }
    }
}
