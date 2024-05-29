using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_BillDetail : DomainEntities.DomainEntities
    {
        public Guid? billId { get; set; } 
        /// <summary>
        /// Khoản thu
        /// </summary>
        public Guid? tuitionConfigId { get; set; }
        [NotMapped]
        public string tuitionConfigName { get; set; }
        /// <summary>
        /// Đơn giá
        /// </summary>
        public double? price { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public double? quantity { get; set; }
        /// <summary>
        /// tổng tiền trước khi giảm
        /// </summary>
        public double? totalPrice { get; set; }
        /// <summary>
        /// Mã giảm giá
        /// </summary>
        public Guid? discountId { get; set; }
        /// <summary>
        /// Loại khuyến mãi tại thời điểm áp dụng
        /// </summary>
        public int? discountType { get; set; }
        [NotMapped]
        public string discountTypeName { get; set; }
        /// <summary>
        /// Giá trị khuyến mãi tại thời điểm áp dụng
        /// </summary>
        public double? discountValue { get; set; }
        /// <summary>
        /// Giá trị khuyến mãi tối đa tại thời điểm áp dụng
        /// </summary>
        public double? discountMaximumValue { get; set; }
        /// <summary>
        /// Đơn giá giảm
        /// </summary>
        public double? reduced { get; set; }
        /// <summary>
        /// Tổng giảm
        /// </summary>
        public double? totalReduced { get; set; }
        /// <summary>
        /// Tồng tiền sau khi đã trừ giảm giá
        /// </summary>
        public double? totalFinalPrice { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string note { get; set; }
    }
}
