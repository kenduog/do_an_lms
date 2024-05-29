using Newtonsoft.Json;
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
    public class GroupPermissionCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_group)]
        public Guid? groupId { get; set; }
        [Required(ErrorMessage = MessageContants.req_permission)]
        public Guid? permissionId { get; set; }
    }
}
