using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Wards : DomainEntities.DomainEntities
    {
        public Guid? districtId { get; set; }
        [NotMapped]
        public string districtName { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
