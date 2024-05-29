using Entities;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IMenuWeekService : IDomainService<tbl_MenuWeek, BaseSearch>
    {
        //Task CustomAddItem(MenuWeekCreate request);
        Task AddOrUpdateItem(MenuWeekUpdate request);
        Task<List<tbl_MenuWeek>> CustomGet(MenuWeekSearch request);
        Task<List<MenuWeekItem>> GetRandom(MenuWeekRandomSearch baseSearch);
        Task SendNotify(tbl_Week week, List<tbl_Users> receivers);
    }
}
