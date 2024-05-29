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
    public class AttendanceCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }

        [Required(ErrorMessage = MessageContants.req_AttendanceDate)]
        public double? date { get; set; }
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        /// <summary>
        /// 1 - Có mặt
        /// 2 - Vắng phép
        /// 3 - Vắng không phép
        /// </summary>
        public int? status { get; set; }
    }

    public class AttendanceNotificationRequest
    {
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationTitle)]
        public string title { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationContent)]
        public string content { get; set; }
    }
}
