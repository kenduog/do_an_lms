using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_PaymentMethod : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public Guid? paymentBankId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string qrCode { get; set; }
        /// <summary>
        /// 1 - chuyển khoản
        /// 2 - khác
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        [NotMapped]
        public string branchName { get; set; }
        public static string GetTypeName(int type) => type == 1 ? "Chuyển khoản" : type == 2 ? "Khác" : "";

    }
}
