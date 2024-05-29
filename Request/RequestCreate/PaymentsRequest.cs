using Entities;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class PaymentsRequest
    {
        /// <summary>
        /// Vui lòng chọn công nợ
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn công nợ")]
        public Guid? billId { get; set; }
        /// <summary>
        /// số tiền
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        public double? value { get; set; }
        /// <summary>
        /// 1 - Thu vào 
        /// 2 - Chi ra
        /// </summary>
        [JsonIgnore]
        public int? type { get; set; } = 1;
        [JsonIgnore]
        public string typeName
        {
            get
            {
                return tbl_PaymentSession.GetTypeName(type ?? 1);
            }
        }
        /// <summary>
        /// ngày thanh toán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày thanh toán")]
        public double? paymentDate { get; set; }    
        /// <summary>
        /// ghi chú
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// Vui lòng phương thức thanh toán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public Guid? paymentMethodId { get; set; }
    }
}
