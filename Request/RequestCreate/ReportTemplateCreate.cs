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
    public class ReportTemplateCreate : DomainCreate
    {
        public string code { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public string tokens { get; set; }
    }
}
