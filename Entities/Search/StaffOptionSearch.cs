using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class StaffOptionSearch 
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_code)]
        public string code { get; set; }
    }
    public class TeacherNotAssignmentOptionSearch
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid schoolYearId { get; set; }
    }
    public class GroupOptionSearch
    {
        public bool? isStaff { get; set; }
    }

}
