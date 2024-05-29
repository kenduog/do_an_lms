using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FeedbackPermission : DomainEntities.DomainEntities
    {
        //mã quyền
        public string code { get; set; }
        [NotMapped]
        public string name { get; set; }
        //nhóm được xử lý
        public string groupIds { get; set; }
        //giáo viên hoặc học viên
        public string roleIds { get; set; }
        // chi nhánh
        public Guid? branchId { get; set; }
        [NotMapped]
        public List<string> groupNames_vi { get; set; }
    }
}
