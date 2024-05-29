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
    public interface IFoodItemService : IDomainService<tbl_FoodItem, FoodItemSearch>
    {
        Task<List<tbl_FoodItem>> GetFoodItemMenu(FoodItemMenuRequest request);
    }
}
