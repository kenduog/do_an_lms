using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class BillSearch : BaseSearch
    {
        /// <summary>
        /// Học viên
        /// </summary>
        public Guid? studentId { get; set; }
        public Guid? branchId { get; set; }
        /// <summary>
        /// 0 - tất cả
        /// 1 - chưa thanh toán hết
        /// 2 - đã thành toán hết
        /// </summary>
        public int? Status { get; set; }
    }
}
