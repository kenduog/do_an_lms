using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Search
{
    public class NutritionGroupSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
    }
}
