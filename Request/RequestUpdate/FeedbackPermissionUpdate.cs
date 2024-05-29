using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class FeedbackPermissionUpdate : DomainUpdate
    {
        public string groupIds { get; set; }
        public string roleIds { get; set; }
    }
}
