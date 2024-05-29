using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class LikeInNewsCreate : DomainCreate
    {
        /// <summary>
        /// Bài đăng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bài đăng")]
        public Guid newsId { get; set; }
    }
}
