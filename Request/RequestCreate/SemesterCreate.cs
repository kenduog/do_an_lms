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
    public class SemesterCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string description { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public double? sTime { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public double? eTime { get; set; }
        public int? weeks { get; set; }

    }
}
