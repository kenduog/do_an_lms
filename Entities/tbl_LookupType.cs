using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_LookupType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Key]
        public string code { get; set; }
        public string name { get; set; }
        public double? created { get; set; }
        public Guid? createdBy { get; set; }
        public double? updated { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? active { get; set; } = true;
    }
}
