using Request.DomainRequests;
using System.Collections.Generic;
using System;

namespace Request.RequestUpdate
{
    public class TeachingAssignmentUpdate : DomainUpdate
    {
        public Guid subjectId { get; set; }
        public List<Guid> teacherIds { get; set; }
    }
}
