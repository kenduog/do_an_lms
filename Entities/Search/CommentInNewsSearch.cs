using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class CommentInNewsSearch : BaseSearch
    {
        /// <summary>
        /// Bài đăng -> Lấy danh sách bình luận trong bài đăng (=> replycommentid = null)
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn bài đăng")]
        public Guid newsId { get; set; }
        /// <summary>
        /// Bình luận được trả lời -> Lấy danh sách reply trong comment (=> replycommentid != null)
        /// </summary>
        public Guid? replyCommentId { get; set; }
    }
}
