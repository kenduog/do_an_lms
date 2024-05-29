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

namespace Request.RequestUpdate
{
    public class CollectionSessionUpdate : DomainUpdate
    {
        public string notificationContent { get; set; }
        public string paymentStruct { get; set; }
    }

    public class CollectionSessionHeaderUpdate : DomainUpdate
    {
        /// <summary>
        /// Miễn giảm
        /// </summary>
        public Guid? feeReductionId { get; set; }
        public string feeReductionName { get; set; }
        public double? otherFee { get; set; }
        public string reasonForOtherFee { get; set; }

        /// <summary>
        /// Tiền hoàn lại theo số buổi có phép
        /// </summary>
        public double? refund { get; set; }
        /// <summary>
        /// Số ngày vắng có phép
        /// </summary>
        public int? daysOfAbsent { get; set; }
        /// <summary>
        /// Đã thanh toán chưa
        /// </summary>
        public bool? isPaid { get; set; } = false;

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public double? paid { get; set; }
        public double? paymentDate { get; set; }
    }

    public class CollectionSessionLineUpdate : DomainUpdate
    {
        public double? value { get; set; }
    }

    public class ComfirmPayment: DomainUpdate
    {
        public Guid? paymentMethodId { get; set; }
    }
}
