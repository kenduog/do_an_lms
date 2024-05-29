﻿using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class VendorUpdate : DomainUpdate
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string description { get; set; }
    }
}
