using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_CollectionSessionFee : DomainEntities.DomainEntities
    {
        public Guid? collectionSessionId { get; set; }
        public Guid? collectionSessionHeaderId { get; set; }
        public Guid? feeId { get; set; }
        public double? value { get; set; }
        /// <summary>
        /// Loại giảm
        /// 1 - Giảm phần trăm
        /// 2 - Giảm tiền
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// Nội dung giảm giá
        /// </summary>
        public string name { get; set; }
    }
}
