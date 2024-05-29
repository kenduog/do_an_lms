using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class SemesterUpdate : DomainUpdate
    {
        public string name { get; set; }
        public string description { get; set; }
        public int? weeks { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
