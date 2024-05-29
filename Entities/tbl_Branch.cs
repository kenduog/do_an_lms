using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Branch : DomainEntities.DomainEntities
    {
        /// <summary>
        /// 1 - Mầm non
        /// 2 - Tiểu học
        /// ...
        /// </summary>
        public int? type { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string logo { get; set; }
        /// <summary>
        /// thông tin logo in trên bill => cái này BE tự chọt xuống DB 
        /// </summary>
        public string logoInBill { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
        public string address { get; set; }
        public Guid? districtId { get; set; }
        public Guid? cityId { get; set; }
        public Guid? wardId { get; set; }
        [NotMapped]
        public string districtName { get; set; }
        [NotMapped]
        public string cityName { get; set; }
        [NotMapped]
        public string wardName { get; set; }
    }
}
