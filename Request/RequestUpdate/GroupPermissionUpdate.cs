using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class GroupPermissionUpdate : DomainUpdate
    {
        public Guid? groupId { get; set; }
        public Guid? permissionId { get; set; }
    }
}
