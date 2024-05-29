using System;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace Request.RequestCreate
{
    public class FeedbackReplyCreate : DomainRequests.DomainCreate
    {
        /// <summary>
        /// Id bài phản hồi
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_feedbackId)]
        public Guid? feedbackId { get; set; }
        /// <summary>
        /// Nội dung nhận xét
        /// </summary>
        public string content { get; set; }
    }
}
