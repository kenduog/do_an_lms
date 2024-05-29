using Entities;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class PaymentMethodCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public Guid? paymentBankId { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string description { get; set; }
        public string qrCode { get; set; }
        /// <summary>
        /// 1 - chuyển khoản
        /// 2 - khác
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại hình thức thanh toán")]
        public int? type { get; set; }
        [JsonIgnore]
        public string typeName
        {
            get
            {
                return tbl_PaymentMethod.GetTypeName(type ?? 0);
            }
        }
    }
}
