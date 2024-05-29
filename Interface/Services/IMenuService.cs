using Entities;
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
    public interface IMenuService : IDomainService<tbl_Menu, MenuSearch>
    {
        Task<PagedList<tbl_Menu>> GetMenuWithFoods(MenuSearch search);
    }
}
