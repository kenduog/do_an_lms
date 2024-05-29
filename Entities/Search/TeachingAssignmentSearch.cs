using Entities.DomainEntities;
using System;

namespace Entities.Search
{
    public class TeachingAssignmentSearch : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? teacherId { get; set; }
        public Guid? subjectId { get; set; }
    }
    public class TeacherBySubjectRequest
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public Guid? subjectId { get; set; }
    }
}
