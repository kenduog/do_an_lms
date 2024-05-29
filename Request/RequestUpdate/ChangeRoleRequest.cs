using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class ChangeRoleRequest
    {
        /// <summary>
        /// Người dùng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn người dùng")]
        public Guid? userId { get; set; }
        /// <summary>
        /// Mã quyền
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn mã quyền")]
        public string roleCode { get; set; }
    }
}
