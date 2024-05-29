using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Holiday : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
