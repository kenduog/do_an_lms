using Entities;
using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class PaymentSessionCreate : DomainCreate
    {
        public Guid? studentId { get; set; }      
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        public Guid? branchId { get; set; }
        /// <summary>
        /// số tiền
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        public double? value { get; set; }
        /// <summary>
        /// 1 - Thu vào 
        /// 2 - Chi ra
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn loại thanh toán")]
        public int? type { get; set; } = 1;
        [JsonIgnore]
        public string typeName
        {
            get
            {
                return tbl_PaymentSession.GetTypeName(type ?? 0);
            }
        }
        [JsonIgnore]
        public int? status { get; set; } = 2;
        [JsonIgnore]
        public string statusName
        {
            get
            {
                return tbl_PaymentSession.GetTypeName(type ?? 0);
            }
        }
        /// <summary>
        /// ngày thanh toán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày thanh toán")]
        public double? paymentDate { get; set; }
        /// <summary>
        /// Vui lòng phương thức thanh toán
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public Guid? paymentMethodId { get; set; }
        /// <summary>
        /// Lý do 
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập lý do")]
        public string reason { get; set; }
        public string note { get; set; }
    }
}
