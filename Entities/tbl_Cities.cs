using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Cities : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
