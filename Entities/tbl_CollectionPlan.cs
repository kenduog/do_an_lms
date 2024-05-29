using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_CollectionPlan : DomainEntities.DomainEntities
    {
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
        /// 1 sắp mở
        /// 2 đang mở
        /// 3 đã đóng
        /// </summary>
        public int? status { get; set; }
        public string statusName { get; set; }
        public Guid? semesterId { get; set; }
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

        [NotMapped]
        public virtual List<tbl_CollectionPlanDetail> details { get; set; }
    }
}
