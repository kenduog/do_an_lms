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
    public class ScaleMeasureDetailUpdate : DomainUpdate
    {
        public double? weight { get; set; }
        public double? height { get; set; }
        public string evaluation { get; set; }
    }
}
