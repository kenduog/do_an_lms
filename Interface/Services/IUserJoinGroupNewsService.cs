using Entities;
using Entities.AuthEntities;
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
    public interface IUserJoinGroupNewsService : IDomainService<tbl_UserJoinGroupNews, UserJoinGroupNewsSearch>
    {
        Task<int> GetTypeUser(Guid groupId, Guid userId);
        Task<string> GetTypeUserName(Guid groupId, Guid userId);
        Task<string> GetGroupName(Guid userId);
        Task<bool> Update(tbl_UserJoinGroupNews item);
        Task Validate(Guid groupNewsId);
    }
}
