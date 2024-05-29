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
    public class ScaleMeasureDetailCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        [Required(ErrorMessage = MessageContants.req_classId)]
        public Guid? classId { get; set; }
        [Required(ErrorMessage = MessageContants.req_scaleMeasureId)]
        public Guid? scaleMeasureId { get; set; }
        [Required(ErrorMessage = MessageContants.req_studentId)]
        public Guid? studentId { get; set; }
        public int? monthOfAge { get; set; }
        public double? weight { get; set; }
        public double? height { get; set; }
        public string evaluation { get; set; }
    }
}
