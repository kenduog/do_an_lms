using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class NotificationCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string title { get; set; }
        public string content { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn người dùng")]
        public Guid? userId { get; set; }
    }
}
