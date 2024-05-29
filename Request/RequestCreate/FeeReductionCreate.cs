using Entities;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class FeeReductionCreate : DomainCreate
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        [NotMapped]
        public virtual List<FeeReductionConfigCreate> items { get; set; }
    }

    public class FeeReductionConfigCreate : DomainCreate
    {
        public Guid? feeId { get; set; }
        /// <summary>
        /// Loại giảm
        /// 1 - Giảm phần trăm
        /// 2 - Giảm tiền
        /// </summary>
        public int? type { get; set; }
        public double? value { get; set; }
    }
}
