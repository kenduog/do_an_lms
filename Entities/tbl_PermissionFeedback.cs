using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_PermissionFeedback : DomainEntities.DomainEntities
    {
        public string roleStaffId { get; set; }
        //nhóm được xử lý
        public string groupIds { get; set; }
        //giáo viên hoặc học viên
        public string roleIds { get; set; }
        [NotMapped]
        public string groupNames { get; set; }
        [NotMapped]
        public string roleNames { get; set; }
        [NotMapped]
        public string roleStaffName { get; set; }
    }
}
