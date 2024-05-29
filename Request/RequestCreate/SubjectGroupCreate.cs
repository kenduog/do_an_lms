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
    public class SubjectGroupCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_schoolYearId)]
        public Guid? schoolYearId { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
    }
}
