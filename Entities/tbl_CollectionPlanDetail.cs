using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_CollectionPlanDetail : DomainEntities.DomainEntities
    {
        public Guid? collectionPlanId { get; set; }
        public Guid? feeId { get; set; }
        [NotMapped]
        public string feeName { get; set; }
        /// <summary>
        /// Kỳ thu:
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; }
        public double? price { get; set; }
        public string description { get; set; }
    }
}
