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
    public interface IWeekService : IDomainService<tbl_Week, WeekSearch>
    {
        Task<AppDomainResult> GenerateWeek(GenerateWeekCreate generate, int number);
        Task<bool> AddWeek(tbl_Week itemModel);
        Task<tbl_Week> UpdateWeek(tbl_Week itemModel);

    }
}
