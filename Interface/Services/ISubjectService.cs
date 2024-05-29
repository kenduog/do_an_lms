using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface ISubjectService : IDomainService<tbl_Subject, SubjectSearch>
    {
        Task<List<SubjectPrepare>> GetSubjectToPrepareTimeTable(SubjectPrepareSearch request);
    }
}
