using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FeeReductionConfig: DomainEntities.DomainEntities
    {
        public Guid? feeReductionId { get; set; }
        public Guid? feeId { get; set; }

        /// <summary>
        /// Loại giảm
        /// 1 - Giảm phần trăm
        /// 2 - Giảm tiền
        /// </summary>
        public int? type { get; set; }
        public double? value { get; set; }

        [NotMapped]
        public string feeName { get; set; }
    }
}
