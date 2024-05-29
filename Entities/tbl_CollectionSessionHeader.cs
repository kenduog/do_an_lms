using Entities.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_CollectionSessionHeader : DomainEntities.DomainEntities
    {
        public tbl_CollectionSessionHeader() { }
        public tbl_CollectionSessionHeader(tbl_CollectionPlan collectionPlan, Guid studentId, Guid collectionSessionId, double? amt) 
        {
            this.studentId = studentId;
            this.collectionSessionId = collectionSessionId;
            this.amt = amt;

            this.collectionPlanId = collectionPlan.id;
            this.branchId = collectionPlan.branchId;
            this.schoolYearId = collectionPlan.schoolYearId;
            this.gradeId = collectionPlan.gradeId;
            this.classId = collectionPlan.classId;
            this.name = collectionPlan.name;
            this.description = collectionPlan.description;
            this.collectionType = collectionPlan.collectionType;
            this.startDay = collectionPlan.startDay;
            this.endDay = collectionPlan.endDay;
            this.scope = collectionPlan.scope;
            this.allowFeeReduction = collectionPlan.allowFeeReduction;
            this.condition = collectionPlan.condition;
        }
        public Guid? collectionPlanId { get; set; }
        [NotMapped]
        public string collectionPlanName { get; set; }
        public Guid? collectionSessionId { get; set; }
        public Guid? studentId { get; set; }
        [NotMapped]
        public string studentFullName { get; set; }

        #region base information
        public Guid? branchId { get; set; }
        public Guid? schoolYearId { get; set; }
        /// <summary>
        /// Trường hợp thu theo khối thì mới có giá trị
        /// </summary>
        public Guid? gradeId { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        /// <summary>
        /// Trường hợp thu theo lớp thì mới có giá trị
        /// </summary>
        public Guid? classId { get; set; }
        [NotMapped]
        public string className { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        /// <summary>
        /// Phát sinh kỳ thu
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; }
        /// <summary>
        /// Ngày bắt đầu phát sinh khoản thu của tháng, quí...
        /// Ví dụ: startDay = 3 thì bắt đầu phát sinh vào ngày 3 hàng tháng, hoặc ngày 3 của tháng bắt đầu quí,...
        /// </summary>
        public int? startDay { get; set; }
        /// <summary>
        /// Hạn chót thanh toán khoản thu. Cơ chế tương tự ngày bắt đầu
        /// </summary>
        public int? endDay { get; set; }
        /// <summary>
        /// Phạm vi thu tiền
        /// 1 - Lớp
        /// 2 - Khối
        /// 3 - Toàn trường
        /// </summary>
        public int? scope { get; set; }
        /// <summary>
        /// Cho phép áp dụng miễn giảm không
        /// </summary>
        public bool? allowFeeReduction { get; set; }
        /// <summary>
        /// Tính chất khoản thu
        /// 1 - Bắt buộc
        /// 2 - Tự nguyện
        /// </summary>
        public int? condition { get; set; }
        #endregion

        /// <summary>
        /// Miễn giảm
        /// </summary>
        public Guid? feeReductionId { get; set; }
        [NotMapped]
        public string feeReductionName { get; set; }
        [NotMapped]
        public virtual List<tbl_CollectionSessionFee> reductionFees { get; set; }

        /// <summary>
        /// Phí khác
        /// </summary>
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
        [NotMapped]
        public double? amt { get 
            {
                double total = 0;
                double totalReduction = 0;
                if(reductionFees != null && reductionFees.Count > 0)
                    totalReduction = reductionFees.Sum(x => (x.value ?? 0));
                double totalFee = 0;
                if (details != null && details.Count > 0)
                    totalFee = details.Sum(x => (x.price ?? 0));
                total = (totalFee - totalReduction + (otherFee ?? 0) - (refund ??0) - (paid  ?? 0));
                return total;
            }
            set { } }

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
        public int? paymentStatus { get; set; } = 1;
        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public double? paid { get; set; } = 0;
        public double? paymentDate { get; set; }
        [NotMapped]
        public DateTime? dateTime { get { if (paymentDate.HasValue) return Timestamp.ToLocalDateTime(paymentDate); return null; } set { } }

        /// <summary>
        /// Số thông báo đã gửi
        /// </summary>
        public int? notificationSent { get; set; }

        [NotMapped]
        public virtual List<tbl_CollectionSessionLine> details { get; set; }

        [NotMapped]
        public int? month { get; set; }
        [NotMapped]
        public int? year { get; set; }
    }

    public class CollectionSessionHeaderMobile : DomainEntities.DomainEntities
    {
        public string paymentStruct { get; set; }
        /// <summary>
        /// Trạng thái thanh toán
        /// 1 - Chưa thanh toán
        /// 2 - Chờ duyệt
        /// 3 - Đã thanh toán
        /// </summary>
        public int? paymentStatus { get; set; } = 1;
        public string collectionPlanName { get; set; }
        public string name { get; set; }
        /// <summary>
        /// Tính chất khoản thu
        /// 1 - Bắt buộc
        /// 2 - Tự nguyện
        /// </summary>
        public int? condition { get; set; } = 1;
        public int? endDay { get; set; }
        /// <summary>
        /// Tổng tiền cần phải thu
        /// </summary>
        public double? amt { get; set; } = 0;

        /// <summary>
        /// Đã thanh toán chưa
        /// </summary>
        public bool? isPaid { get; set; } = false;

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public double? paid { get; set; } = 0;
        public double? debt { get { return Math.Max((amt ?? 0) - (paid ?? 0), 0); } set { } }
        public double? paymentDate { get; set; }

        public int? month { get; set; }
        public int? year { get; set; }
        /// <summary>
        /// Tiền hoàn lại theo số buổi có phép
        /// </summary>
        public double? refund { get; set; } = 0;
        /// <summary>
        /// Số ngày vắng có phép
        /// </summary>
        public int? daysOfAbsent { get; set; } = 0;
        /// <summary>
        /// Phí khác
        /// </summary>
        public double? otherFee { get; set; } = 0;
        public string reasonForOtherFee { get; set; }
    }

    public class CollectionSessionHeaderMobileModel : DomainEntities.DomainEntities
    {
        public string paymentStruct { get; set; }
        public string collectionPlanName { get; set; }
        public string name { get; set; }
        /// <summary>
        /// Tổng tiền cần phải thu
        /// </summary>
        public double? amt { get; set; } = 0;
        /// <summary>
        /// Đã thanh toán chưa
        /// </summary>
        public bool? isPaid { get; set; } = false;
        public double? paid { get; set; } = 0;
        public virtual List<CollectionKeyValue> fees { get; set; }
        public virtual List<CollectionKeyValueDescription> otherFees { get; set; }
        public virtual List<CollectionKeyValueReduction> reductions { get; set; }
    }
    public class CollectionKeyValue
    {
        public string name { get; set; }
        public double? value { get; set; }
    }
    public class CollectionKeyValueDescription
    {
        public string name { get; set; }
        public string description { get; set; }
        public double? value { get; set; }
    }
    public class CollectionKeyValueReduction
    {
        public string name { get; set; }
        public double? value { get; set; }
    }
}
