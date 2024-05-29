using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class SubjectGroupSearch : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
    }
}
