using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FeedbackReply : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Id bài phản hồi
        /// </summary>
        public Guid? feedbackId { get; set; }
        /// <summary>
        /// Nội dung nhận xét
        /// </summary>
        public string content { get; set; }

        [NotMapped]
        public string createdByName { get; set; }
        [NotMapped]
        public string thumbnail { get; set; }
        [NotMapped]
        public string thumbnailResize { get; set; }
        [NotMapped]
        public bool owner { get; set; } = false;
    }
}
