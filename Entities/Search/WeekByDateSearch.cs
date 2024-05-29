using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class WeekByDateSearch
    {
        public double? date { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? semesterId { get; set; }

    }
}
