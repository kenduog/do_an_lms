using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_CollectionSession : DomainEntities.DomainEntities
    {
        public Guid? collectionPlanId { get; set; }
        public string name { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }
        public string description { get; set; }
        public double? amt { get; set; }
        public string notificationContent { get; set; }
        public string paymentStruct { get; set; }
        [NotMapped]
        public string collectionPlanName { get; set; }
        [NotMapped]
        public int? status { get; set; }
        [NotMapped]
        public double? debt { get { return Math.Max((amt ?? 0) - (paid ?? 0), 0); } set { } }
        [NotMapped]
        public double? paid { get; set; }
        [NotMapped]
        public PagedList<CollectionSessionItem> collectionSessionItems { get; set; }
    }
    public class CollectionSessionItem
    {
        public Guid? id { get; set; }
        public Guid? studentId { get; set; }
        public string studentFullName { get; set; }
        public string className { get; set; }
        public Dictionary<Guid, CollectionSessionSubItem> fees { get; set; }

        /// <summary>
        /// Miễn giảm
        /// </summary>
        public Guid? feeReductionId { get; set; }
        public string feeReductionName { get; set; }
        public virtual List<tbl_CollectionSessionFee> reductionFees { get; set; }

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
        /// Tổng tiền cần phải thu
        /// </summary>
        public double? amt
        {
            get
            {
                double total = 0;
                var totalReduction = reductionFees.Sum(x => (x.value ?? 0));
                var totalFee = fees.Sum(x => (x.Value.price ?? 0));
                total = (totalFee - totalReduction + (otherFee ?? 0) - (refund ?? 0) - (paid ?? 0));
                return total;
            }
            set { }
        }
        /// <summary>
        /// Đã thanh toán chưa
        /// </summary>
        public bool? isPaid { get; set; } = false;
        /// <summary>
        /// Trạng thái thanh toán
        /// 1 - Chưa thanh toán
        /// 2 - Chờ duyệt
        /// 3 - Đã thanh toán
        /// </summary>
        public int? paymentStatus { get; set; }
        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public double? paid { get; set; } = 0;
    }

    public class CollectionSessionSubItem
    {
        public Guid id { get; set; }
        public string feeName { get; set; }
        public double? price { get; set; }
    }
}
