using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Request.Auth
{
    public class Login
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc nhập")]
        public string username { set; get; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc nhập")]
        public string password { set; get; }
    }
}
