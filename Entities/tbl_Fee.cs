using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Fee: DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        /// <summary>
        /// Kỳ thu:
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; }

        [NotMapped]
        public string gradeNames { get; set; }
        [NotMapped]
        public double? price{ get; set; }
        [NotMapped]
        public virtual List<tbl_FeeInGrade> items { get; set; }
    }
}
