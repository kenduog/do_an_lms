using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Necessary : DomainEntities.DomainEntities
    {
        public int? type { get; set; }
        public string typeName { get; set; }
        /// <summary>
        /// type 1 - Hình thức thanh toán(tbl_PaymentMethod)
        /// type 2 - Mẫu(tbl_SampleConfig), mẫu các phiếu thu chi ...
        /// </summary>
        public string content { get; set; }
    }
}
