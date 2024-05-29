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

namespace Request.RequestUpdate
{
    public class GoodBehaviorCertificateUpdate : DomainUpdate
    {
        [Required(ErrorMessage = "Vui lòng chọn trạng thái đầy đủ!")]
        public bool? status { get; set; }
        public string note { get; set; } 
    }

    public class GoodBehaviorNotificationRequest
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationTitle)]
        public string title { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationContent)]
        public string content { get; set; }
    }
}
