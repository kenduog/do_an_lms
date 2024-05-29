using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_CommentInNews : DomainEntities.DomainEntities
    {
        public Guid? newsId { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// Người bình luận
        /// </summary>
        public Guid? userId { get; set; }
        /// <summary>
        /// Bình luận được trả lời
        /// </summary>
        public Guid? replyCommentId { get; set; }
        [NotMapped]
        public string fullName { get; set; }
        [NotMapped]
        public string userThumnail { get; set; }
        [NotMapped]
        public int countReply { get; set; }

    }
}
