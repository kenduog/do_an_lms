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
    public class BranchCreate : DomainCreate
    {

        /// <summary>
        /// 1 - Mầm non
        /// 2 - Tiểu học
        /// ...
        /// </summary>
        public int? type { get; set; }
        public string code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên chi nhánh")]
        public string name { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string logo { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
        public string address { get; set; }
        public Guid? districtId { get; set; }
        public Guid? cityId { get; set; }
        public Guid? wardId { get; set; }
    }
}
