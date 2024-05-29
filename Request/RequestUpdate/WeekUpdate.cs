using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class WeekUpdate : DomainUpdate
    {
        public string name { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
