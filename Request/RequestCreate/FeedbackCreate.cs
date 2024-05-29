using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestCreate
{
    public class FeedbackCreate : DomainCreate
    {
        /// <summary>
        /// Id nhóm phản hồi
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_feedbackGroupId)]
        public Guid? feedbackGroupId { get; set; }
        /// <summary>
        /// Tiêu đề phản hồi
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_title)]
        public string title { get; set; }
        /// <summary>
        /// Nội dung phản hồi
        /// </summary>
        public string content { get; set; }
        [JsonIgnore]
        public int status = 1;
    }
}
