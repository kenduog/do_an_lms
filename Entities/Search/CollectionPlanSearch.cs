using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class CollectionPlanSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? semesterId { get; set; }
        /// <summary>
        /// Trường hợp thu theo khối thì mới có giá trị
        /// </summary>
        public Guid? gradeId { get; set; }
        /// <summary>
        /// Trường hợp thu theo lớp thì mới có giá trị
        /// </summary>
        public Guid? classId { get; set; }
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
    }

    public class CollectionPlanReport 
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        /// <summary>
        /// Loại thống kê
        /// 1 - Theo ngày (tối đa 30 ngày)
        /// 2 - Theo tuần (tối đa 24 tuần)
        /// 3 - Theo tháng 
        /// 4 - Theo quý
        /// 5 - Theo năm
        /// </summary>
        public int? type { get; set; }
    }

    public class AverageMoneyPerStudent
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        /// <summary>
        /// Loại thống kê
        /// 1 - Theo ngày (tối đa 30 ngày)
        /// 2 - Theo tuần (tối đa 24 tuần)
        /// 3 - Theo tháng 
        /// 4 - Theo quý
        /// 5 - Theo năm
        /// </summary>
        public int? type { get; set; }
    }

    public class CollectionSessionReport
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? collectionPlanId { get; set; }
    }
    public class ReportByFee
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }

}
