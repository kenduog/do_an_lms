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
    public interface IGroupService : IDomainService<tbl_Group, BaseSearch>
    {
        Task<tbl_Group> GetByCode(string code);
    }
}
