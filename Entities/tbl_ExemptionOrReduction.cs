using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ExemptionOrReduction : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        /// <summary>
        /// 1 - Miễn học phí
        /// 2 - Giảm học phí theo phần trăm
        /// 3 - Giảm học phí theo số tiền
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        public double? value { get; set; }
    }
}
