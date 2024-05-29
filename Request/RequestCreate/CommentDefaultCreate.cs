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
    public class CommentDefaultCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required(ErrorMessage = "Vui lòng ghi tiêu đề!")]
        public string title { get; set; }
        public string lunch { get; set; }
        public string afternoonSnack { get; set; }
        public string afternoon { get; set; }
        public string sleep { get; set; }
        public string hygiene { get; set; }
    }
}
