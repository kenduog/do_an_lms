using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface ITeachingAssignmentService : IDomainService<tbl_TeachingAssignment, TeachingAssignmentSearch>
    {
        Task<List<TeacherBySubjectReponse>> GetTeacherAssignmentBySubject(TeacherBySubjectRequest request);
        Task AddOrUpdate(TeachingAssignmentCreate request);
    }
}
