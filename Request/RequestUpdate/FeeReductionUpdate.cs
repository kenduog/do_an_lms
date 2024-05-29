using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class FeeReductionUpdate : DomainUpdate
    {
        public string name { get; set; }
        public string description { get; set; }

        [NotMapped]
        public virtual List<FeeReductionConfigUpdate> items { get; set; }
    }

    public class FeeReductionConfigUpdate: DomainUpdate
    {
        /// <summary>
        /// Loại giảm
        /// 1 - Giảm phần trăm
        /// 2 - Giảm tiền
        /// </summary>
        public int? type { get; set; }
        public double? value { get; set; }
    }
}
