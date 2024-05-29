using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestCreate
{
    public class ScaleMeasureCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_scaleMeasureId)]
        public string name { get; set; }
        [Required(ErrorMessage = MessageContants.req_scaleMeasureDate)]
        public double date { get; set; }
        /// <summary>
        /// 1 = Toàn trường (toàn bộ lớp của trường)
        /// 2 = Khối (Chọn các lớp của khối)
        /// 3 = Lớp (Chọn 1 lớp duy nhất)
        /// </summary>
        public int? type { get; set; }
        public Guid? branchId { get; set; }
        public Guid? schoolYearId { get; set; }

        public string gradeIds { get; set; }
        public string classIds { get; set; }
    }

    public class ScaleMeasureNotificationRequest
    {
        [Required(ErrorMessage = MessageContants.req_scaleMeasureId)]
        public Guid? scaleMeasureId { get; set; }
    }
}
