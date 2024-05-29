using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class StatisticalSearchV2 : BaseSearch
    {
        /// <summary>
        /// 1 - ngày
        /// 2 - tuần
        /// 3 - tháng
        /// 4 - quý
        /// 5 - năm
        /// 6 - tùy chọn
        /// </summary>
        public int? type { get; set; }
        public Guid? branchId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
