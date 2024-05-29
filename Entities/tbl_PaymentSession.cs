using AutoMapper.Configuration.Conventions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_PaymentSession : DomainEntities.DomainEntities
    {
        public Guid? billId { get; set; }
        /// <summary>
        /// Mã phiếu
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Id học viên
        /// </summary>
        public Guid? studentId { get; set; }
        /// <summary>
        /// Số tiền
        /// </summary>
        public double? value { get; set; }
        /// <summary>
        /// 1 - Thu vào 
        /// 2 - Chi ra
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        /// <summary>
        /// Lý do 
        /// </summary>
        public string reason { get; set; }
        public string note { get; set; }
        public double? paymentDate { get; set; }
        public Guid? branchId { get; set; }
        public static string GetTypeName(int type) => type == 1 ? "Thu vào" : type == 2 ? "Chi ra" : "";
        public Guid? paymentMethodId { get; set; }
        public string printContent { get; set; }
        /// <summary>
        /// 1 - chờ duyệt
        /// 2 - đã duyệt
        /// 3 - không duyệt
        /// </summary>
        public int? status { get; set; }
        public string statusName { get; set; }
        public static string GetStatusName(int status) => status == 1 ? "Chờ duyệt" : status == 2 ? "Đã duyệt" : status == 3 ? "Không duyệt" : "";
        [NotMapped]
        public string studentName { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentThumbnail { get; set; }
        [NotMapped]
        [JsonIgnore]
        public double? totalRevenue { get; set; }
        [NotMapped]
        [JsonIgnore]
        public double? totalExpense { get; set; }
        [NotMapped]
        [JsonIgnore]
        public double? totalProfit { get; set; }
        [NotMapped]
        public string paymentMethodName { get; set; }
    }
    public class PaymentSessionPagedList : PagedList<tbl_PaymentSession>
    {
        /// <summary>
        /// tổng doanh thu
        /// </summary>
        public double? totalRevenue { get; set; }    
        /// <summary>
        /// tổng chi phí
        /// </summary>
        public double? totalExpense { get; set; }
        /// <summary>
        /// tổng lợi nhuận
        /// </summary>
        public double? totalProfit { get; set; }
    }
}
