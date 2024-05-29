using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace Request.RequestCreate
{
    public class FeedbackGroupCreate : DomainCreate
    {
        /// <summary>
        /// Tên nhóm phản hồi
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name_vi { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
    }
}
