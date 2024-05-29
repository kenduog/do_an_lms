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
    public class HolidayCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public double? sTime{ get; set; }
        [LargerStartTime]
        public double? eTime{ get; set; }
    }
}
