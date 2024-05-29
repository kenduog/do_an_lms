using Microsoft.AspNetCore.Http;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class GroupNewsCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng nhập tên nhóm")]
        public string name { get; set; }
        /// <summary>
        /// Danh sách lớp
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public string classIds { get; set; }
        public string background { get; set; }
    }
}
