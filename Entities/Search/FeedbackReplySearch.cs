using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utilities;

namespace Entities.Search
{
    public class FeedbackReplySearch : BaseSearch
    {
        /// <summary>
        /// Lọc theo id bài phản hồi
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_feedbackId)]
        public Guid? feedbackId { get; set; }
    }
}
