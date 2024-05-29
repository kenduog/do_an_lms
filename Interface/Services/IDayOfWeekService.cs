using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IDayOfWeekService : IDomainService<tbl_DayOfWeek, BaseSearch>
    {
        Task<tbl_DayOfWeek> GetByKeyAsync(int key);
    }
}
