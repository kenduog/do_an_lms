using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class CollectionPlanCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = MessageContants.req_semester)]
        public Guid? semesterId { get; set; }
        public int? status { get; set; }
        /// <summary>
        /// Trường hợp thu theo khối thì mới có giá trị
        /// </summary>
        public Guid? gradeId { get; set; }
        /// <summary>
        /// Trường hợp thu theo lớp thì mới có giá trị
        /// </summary>
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string description { get; set; }
        /// <summary>
        /// Phát sinh kỳ thu
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; } = 1;
        /// <summary>
        /// Ngày bắt đầu phát sinh khoản thu của tháng, quí...
        /// Ví dụ: startDay = 3 thì bắt đầu phát sinh vào ngày 3 hàng tháng, hoặc ngày 3 của tháng bắt đầu quí,...
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_startDay)]
        public int? startDay { get; set; }
        /// <summary>
        /// Hạn chót thanh toán khoản thu. Cơ chế tương tự ngày bắt đầu
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_endDay)]
        public int? endDay { get; set; }
        /// <summary>
        /// Phạm vi thu tiền
        /// 1 - Lớp
        /// 2 - Khối
        /// 3 - Toàn trường
        /// </summary>
        public int? scope { get; set; } = 2;
        /// <summary>
        /// Cho phép áp dụng miễn giảm không
        /// </summary>
        public bool? allowFeeReduction { get; set; } = true;
        /// <summary>
        /// Tính chất khoản thu
        /// 1 - Bắt buộc
        /// 2 - Tự nguyện
        /// </summary>
        public int? condition { get; set; } = 1;
        public List<CollectionPlanDetailCreate> details { get; set; }
    }

    public class CollectionPlanDetailCreate : DomainCreate
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
        public double?  price { get; set; }
    }
}
