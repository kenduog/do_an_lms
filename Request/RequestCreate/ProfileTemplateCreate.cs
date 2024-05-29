using Newtonsoft.Json;
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
    public class ProfileTemplateCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public int? type { get; set; }
        [JsonIgnore]
        public int? index { get; set; }
    }
}
