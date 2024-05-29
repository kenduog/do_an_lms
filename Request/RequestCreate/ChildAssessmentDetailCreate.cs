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
    public class ChildAssessmentDetailCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn chủ đề đánh giá!")]
        public Guid? childAssessmentId { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên nội dung đánh giá!")]
        public string name { get; set; }
    }
}
