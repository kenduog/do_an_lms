using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model.Statistical
{
    public class BillInDebtModel
    {
        /// <summary>
        /// tên học viên
        /// </summary>
        public string studentName { get; set; }
        /// <summary>
        /// mã học viên
        /// </summary>
        public string studentCode { get; set; }
        /// <summary>
        /// ảnh đại diện
        /// </summary>
        public string studentThumbnail { get; set; }
        /// <summary>
        /// mã bill
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Tồng tiền phải trả
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
        /// Ghi chú
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// Thời gian thu
        /// </summary>
        public double? date { get; set; }       
    }
}
