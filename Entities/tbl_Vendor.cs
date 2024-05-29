using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Vendor: DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string description { get; set; }
    }
}
