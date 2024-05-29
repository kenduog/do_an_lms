using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Bill : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        /// <summary>
        /// Tổng tiền
        /// </summary>
        public double? totalPrice { get; set; }
        /// <summary>
        /// Tồng tiền được giảm
        /// </summary>
        public double? totalReduced { get; set; }
        /// <summary>
        /// Tồng tiền sau khi đã trừ giảm giá
        /// </summary>
        public double? totalFinalPrice { get; set; }
        /// <summary>
        /// Đã trả
        /// </summary>
        public double? paid { get; set; }
        /// <summary>
        /// Còn nợ
        /// </summary>
        public double? debt { get; set; }
        /// <summary>
        /// Học viên
        /// </summary>
        public Guid? studentId { get; set; }
        public Guid? branchId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// Thời gian thu
        /// </summary>
        public double? date { get; set; }
        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public double? paymentDate { get; set; }
        /// <summary>
        /// 1 - chưa thanh toán
        /// 2 - đang chờ duyệt
        /// 3 - chưa thanh toán hết
        /// 4 - đã thanh toán
        /// </summary>
        public int? status { get; set; }
        public string statusName { get; set; }
        public static string GetStatusName(int status) => status == 1 ? "Chưa thanh toán" : status == 2 ? "Đang chờ duyệt" : status == 3 ? "Chưa thanh toán hết" : status == 4 ? "Đã thanh toán" : "";
        [NotMapped]
        public string studentName { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentThumbnail { get; set; }
        [NotMapped]
        public string createdByName { get; set; }
    }
    //dùng để lấy danh sách lớp theo khối khi tạo đợt thu
    public class ClassByGradeModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string gradeName { get; set; }
    }
    //dùng để lấy danh sách học viên theo lớp khi tạo đợt thu
    public class StudentByClassModel
    {
        public Guid id { get; set; }
        public string thumbnail { get; set; } 
        public string code { get; set; }
        public string fullName { get; set; }
        public string className { get; set; }
        public string gradeName { get; set; }
        
    }
}
