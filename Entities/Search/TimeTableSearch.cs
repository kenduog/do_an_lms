using Entities.DomainEntities;
using System;

namespace Entities.Search
{
    public class TimeTableSearch : BaseSearch
    {
        public Guid? schoolYearId { get; set; }
        public Guid? semesterId  { get; set; }
        public Guid? branchId { get; set; }
        public Guid? gradeId { get; set; }
    }
}
