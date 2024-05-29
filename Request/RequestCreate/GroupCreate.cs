using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class GroupCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên quyền")]
        public string name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã quyền")]
        public string code { get; set; }
    }
}
