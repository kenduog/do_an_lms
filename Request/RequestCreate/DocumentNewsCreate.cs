using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Request.RequestCreate
{
    public class DocumentNewsCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn bài viết")]
        public Guid newsId { get; set; }
        public Guid? commentId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(30 * 1024 * 1024)]
        public IFormFile link { get; set; }
    }
}
