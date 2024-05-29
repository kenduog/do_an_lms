using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_CriteriaDetail : DomainEntities.DomainEntities
    {
        public Guid? criteriaId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}
