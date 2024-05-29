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
    public class GradeCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required (ErrorMessage = MessageContants.req_code)]
        public string code { get; set; }
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }

        /// <summary>
        /// từ viết tắt cho tên lớp
        /// </summary>
        public string acronym { get; set; }
        public int? level { get; set; }
        public int? studentYearOld { get; set; }
    }
}
