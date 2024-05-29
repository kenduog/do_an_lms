using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class CollectionPlanUpdate : DomainUpdate
    {
        public string name { get; set; }
        public int? status { get; set; }
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
        public List<CollectionPlanDetailUpdate> details { get; set; }
    }

    public class CollectionPlanDetailUpdate : DomainUpdate
    {
        public Guid? feeId { get; set; }
        /// <summary>
        /// Kỳ thu:
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; }
        public string description { get; set; }
        public double? price { get; set; }
    }

    public class CollectionPlanUpdateStatus : DomainUpdate
    {
        [Required]
        public int status { get; set; }
    }
}
