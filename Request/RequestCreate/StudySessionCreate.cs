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
    public class StudySessionCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        /// <summary>
        /// Tiết học (từ 1 tới 12)
        /// </summary>
        [Range(1, 12)]
        public int? index { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
