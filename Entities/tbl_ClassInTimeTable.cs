using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ClassInTimeTable : DomainEntities.DomainEntities
    {
        public Guid? classId { get; set; }
        public Guid? timeTableId { get; set; }
    }
}
