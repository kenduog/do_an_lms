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
    public class CriteriaDetailCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_criteriaId)]
        public Guid? criteriaId { get; set; }
        [Required (ErrorMessage = MessageContants.req_code)]
        public string code { get; set; }
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
    }
}
