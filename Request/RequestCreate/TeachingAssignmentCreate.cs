using Request.DomainRequests;
using System;
using System.Collections.Generic;

namespace Request.RequestCreate
{
    public class TeachingAssignmentCreate : DomainCreate
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public List<Guid> teacherIds { get; set; }
        public Guid subjectId { get; set; }
    }
}
