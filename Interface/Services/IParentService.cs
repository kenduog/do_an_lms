using Entities;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IParentService : IDomainService<tbl_Parent, ParentSearch>
    {
        Task<List<StudentByParent>> GetStudentByParent(Guid parentId);
        Task<ParentModel> GetParent(Guid parentId);
        Task<List<tbl_Users>> GetParentUserByStudentId(List<Guid> studentIds);
        Task<List<tbl_Users>> GetParentUserByBranchId(Guid branchId);
    }
}
