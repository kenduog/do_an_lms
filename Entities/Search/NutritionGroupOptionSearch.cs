using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Search
{
    public class NutritionGroupOptionSearch 
    {
        public Guid? branchId { get; set; }
        public Guid? gradeId { get; set; }
    }
}
