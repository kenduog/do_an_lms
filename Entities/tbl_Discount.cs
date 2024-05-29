using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Discount : DomainEntities.DomainEntities
    {
        public Guid? tuitionConfigId { get; set; }
        [NotMapped]
        public string tuitionConfigName { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 1 - Giảm tiền
        /// 2 - Giảm phần trăm
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        /// <summary>
        /// Giá trị
        /// </summary>
        public double? value { get; set; }
        /// <summary>
        /// Giá trị tối đa => áp dụng cho loại %
        /// </summary>
        public double? maximumValue { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string description { get; set; }
        public static string GetTypeName(int type) => type == 1 ? "Giảm tiền" : type == 2 ? "Giảm phần trăm" : ""; 
    }
}
