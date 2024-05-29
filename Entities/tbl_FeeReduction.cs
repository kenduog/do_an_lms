using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FeeReduction: DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        [NotMapped]
        public virtual List<tbl_FeeReductionConfig> items { get; set; }
    }
}
