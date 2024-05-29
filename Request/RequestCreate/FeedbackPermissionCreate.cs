using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class FeedbackPermissionCreate : DomainCreate
    {
        //group code
        public string code { get; set; }
        //nhóm được xử lý
        public string groupIds { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        // chi nhánh
        public Guid branchId { get; set; }
    }
}
