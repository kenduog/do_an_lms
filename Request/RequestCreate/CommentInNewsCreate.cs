using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class CommentInNewsCreate : DomainCreate
    {
        /// <summary>
        /// Bài đăng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bài đăng")]
        public Guid? newsId { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string content { get; set; }
        /// <summary>
        /// Bình luận được trả lời
        /// </summary>
        public Guid? replyCommentId { get; set; }
    }
}
