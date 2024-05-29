using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entities
{
    /// <summary>
    /// Cấu hình khoản thu
    /// </summary>
    public class tbl_TuitionConfig : DomainEntities.DomainEntities
    {
        public Guid? tuitionConfigCategoryId { get; set; }
        [NotMapped]
        public string tuitionConfigCategoryName { get; set; }
        public string name { get; set; }
        public string thumbnail { get; set; }
        public double? price { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// mô tả
        /// </summary>
        public string description { get; set; }
    }
}
