using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class DocumentNewsUpdate : DomainUpdate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên phòng ban")]
        public string name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        public Guid branchid { get; set; }
    }
}
