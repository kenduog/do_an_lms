using Entities;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IGroupNewsService : IDomainService<tbl_GroupNews, BaseSearch>
    {
        Task<bool> UserJoinGroupForClass(GroupNewsCreate request, tbl_GroupNews group);
        Task<PagedList<tbl_Users>> GetUserGroupNews(UserJoinGroupNewsSearch baseSearch);
        Task<bool> GrantPermissions(GrantPermissionsGroupNewsCreate model);
        Task<bool> ChangeOwner(ChangeOwnerGroupNewsCreate model);
        Task<bool> RevokePermissions(ChangeOwnerGroupNewsCreate model);
        Task<bool> CheckAdmin(CheckAdminGroupNewsCreate model);
    }
}
