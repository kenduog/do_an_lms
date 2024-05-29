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
    public class TeachingFrameCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId{ get; set; }
        [Required (ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        [Required (ErrorMessage = MessageContants.req_subjectId)]
        public Guid? subjectId { get; set; }
        /// <summary>
        /// Class per semester 
        /// </summary>
        public int? cps { get; set; }
        /// <summary>
        /// Class per week
        /// </summary>
        public int? cpw { get; set; }
    }
}
