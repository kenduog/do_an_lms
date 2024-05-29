using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.Auth
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string username { get; set; }
    }

    public class MobileForgotPasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string username { get; set; }
    }
}
