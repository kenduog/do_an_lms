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
    public class CriteriaCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        public string description { get; set; }
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
    }
}
