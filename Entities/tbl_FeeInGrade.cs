using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FeeInGrade : DomainEntities.DomainEntities
    {
        public Guid? feeId { get; set; }
        public Guid? gradeId { get; set; }
        public double? price { get; set; }
        [NotMapped]
        public string name { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        [NotMapped]
        public string description { get; set; }
        /// <summary>
        /// Kỳ thu:
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        [NotMapped]
        public int? collectionType { get; set; }
    }
}
