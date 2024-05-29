using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class EditRoleUser
    {
        [Required(ErrorMessage = "Không tìm thấy nhân viên")]
        public Guid userId { get; set; }
        [Required(ErrorMessage = "Không được phép bỏ trống quyền")]
        public string roleCodes { get; set; }
    }
}
