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

namespace Request.RequestUpdate
{
    public class ScaleMeasureUpdate : DomainUpdate
    {
        [Required(ErrorMessage =MessageContants.req_name)]
        public string name { get; set; }
        public double date { get; set; }
    }
}
