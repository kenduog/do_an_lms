using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Vui lòng truyền mãi kích hoạt token")]
        public Guid? refreshToken { get; set; }
    }
}
