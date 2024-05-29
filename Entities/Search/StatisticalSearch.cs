using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class StatisticalSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public int? month { get; set; } = DateTime.Now.Month;
        public int? year { get; set; } = DateTime.Now.Year;     
    }
}
