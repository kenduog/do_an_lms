using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ClassLevel : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
