using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class ScaleMeasureSearch : BaseSearch
    {
        public int? type { get; set; }
        public double? fromDate { get; set; }
        public double? toDate { get; set; }
        public Guid? branchId { get; set; }
        public Guid? schoolYearId { get; set; }
        /// <summary>
        /// Id của bé được chọn - Dùng cho mobile app
        /// </summary>
        public Guid? studentId { get; set; }
    }
}
