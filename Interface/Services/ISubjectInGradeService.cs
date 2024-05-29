using Entities;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface ISubjectInGradeService : IDomainService<tbl_SubjectInGrade, SubjectInGradeSearch>
    {
        Task AddRangeAsync(SubjectInGradeCreate request);
        Task<List<tbl_SubjectInGrade>> GetAllSubjectInGrade(SubjectInGradeSearch request);
    }
}
