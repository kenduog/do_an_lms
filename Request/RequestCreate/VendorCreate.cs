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
    public class VendorCreate : DomainCreate
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string description { get; set; }
    }
}
