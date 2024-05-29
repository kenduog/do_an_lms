using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class ArrangeClassRequest
    {
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_classInfo)]
        public List<ArrangeClassCreate> classes { get; set; }
    }
    public class ArrangeClassCreate
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid classId { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập tên")]
        //public string name { get; set; }
        //public int size { get; set; }
        //[Required(ErrorMessage = "Vui long chọn khối")]
        //public Guid? gradeId { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn giáo viên")]
        //public Guid? teacherId { get; set; }
        [Required(ErrorMessage = MessageContants.req_selectStudent)]
        public List<Guid> studentIds { get; set; }
    }
}
