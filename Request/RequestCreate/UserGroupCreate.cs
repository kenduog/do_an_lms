using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class UserGroupCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_group)]
        public Guid? groupId { get; set; }
        [Required(ErrorMessage = MessageContants.req_user)]
        public Guid? userId { get; set; }
    }
}
