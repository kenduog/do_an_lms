using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_LikeInNews : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Bài đăng
        /// </summary>
        public Guid newsId { get; set; }
        /// <summary>
        /// Bình luận
        /// </summary>
        public Guid? commentId { get; set; }
        /// <summary>
        /// Người đã thích
        /// </summary>
        public Guid? userId { get; set; }
        public bool? isLike { get; set; }
    }
}
