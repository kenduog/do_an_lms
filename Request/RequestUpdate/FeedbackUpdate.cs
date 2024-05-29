using Request.DomainRequests;
using System;

namespace Request.RequestCreate
{
    public class FeedbackUpdate : DomainUpdate
    {
        /// <summary>
        /// Id nhóm phản hồi
        /// </summary>
        public Guid? feedbackGroupId { get; set; }
        /// <summary>
        /// Tiêu đề phản hồi
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Nội dung phản hồi
        /// </summary>
        public string content { get; set; }
    }

    public class FeedbackVote: DomainUpdate
    {
        /// <summary>
        /// Số sao
        /// </summary>
        public int? numberOfStars { get; set; }
    }
}
