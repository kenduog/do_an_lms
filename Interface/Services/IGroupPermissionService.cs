using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IGroupPermissionService : IDomainService<tbl_GroupPermission, BaseSearch>
    {
        Task GrantAllPermissionToGroup(DomainUpdate item);
        Task RemoveAllPermissionToGroup(DomainUpdate item);
    }
}
