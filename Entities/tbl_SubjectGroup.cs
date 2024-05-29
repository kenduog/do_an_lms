using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_SubjectGroup : DomainEntities.DomainEntities
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public string name { get; set; }
    }
}
