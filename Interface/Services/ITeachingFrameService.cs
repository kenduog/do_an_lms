using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface ITeachingFrameService : IDomainService<tbl_TeachingFrame, TeachingFrameSearch>
    {
        Task<PagedList<TeachingFrameGroupByGrade>> GetTeachingFrameGroupByGrade(TeachingFrameGroupByGradeSearch baseSearch);
    }
}
