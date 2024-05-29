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
    public class UserJoinGroupNewsMultipleCreate : DomainCreate
    {
        /// <summary>
        /// Tên nhóm
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn nhóm bảng tin")]
        public Guid groupNewsId { get; set; }
        /// <summary>
        /// Thành viên
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn người tham gia")]
        public string userIds { get; set; }
    }
}
