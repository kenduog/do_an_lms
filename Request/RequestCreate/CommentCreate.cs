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
    public class CommentCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn học sinh")]
        public Guid? studentId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận xét!")]
        public double? date { get; set; }
        public string lunch { get; set; }
        public string afternoonSnack { get; set; }
        public string afternoon { get; set; }
        public string sleep { get; set; }
        public string hygiene { get; set; }
    }

    public class CommentNotificationRequest
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationTitle)]
        public string title { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationContent)]
        public string content { get; set; }
    }
}
