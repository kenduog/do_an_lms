using Microsoft.AspNetCore.Http;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class NewsCreate : DomainCreate
    {
        public Guid? groupNewsId { get; set; } = null;
        public string title { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string content { get; set; }
        /// <summary>
        /// Chọn nhiều chi nhánh dạng 1,2,3
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh đăng bài")]
        public string branchIds { get; set; }
        public List<string> uploads { get; set; }

    }
}
