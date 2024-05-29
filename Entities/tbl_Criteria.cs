using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Criteria : DomainEntities.DomainEntities
    {
        public Guid? gradeId { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
