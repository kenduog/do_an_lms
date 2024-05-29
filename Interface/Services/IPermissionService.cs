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
    public interface IPermissionService : IDomainService<tbl_Permission, PermissionSearch>
    {
        Task<List<tbl_Permission>> GetAction(string code, Guid? userId, bool isSuperUser, string selectedGroupCode);
        Task<List<tbl_Permission>> GetPermission(Guid? userId, string controller, string action);
    }
}
