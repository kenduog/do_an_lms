using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class FoodItemSearch : BaseSearch
    {
        public Guid? foodId { get; set; }
    }

    public class FoodItemMenuRequest 
    {
        public string foodIds { get; set; }
    }

}
