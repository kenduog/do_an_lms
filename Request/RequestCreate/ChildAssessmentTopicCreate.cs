using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestCreate
{
    public class ChildAssessmentTopicCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên chủ đề!")]
        public string name { get; set; }
    }
}
