using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// active - ngày đi học trong tuần
    /// </summary>
    public class tbl_DayOfWeek : DomainEntities.DomainEntities
    {
        public int? key { get; set; }
        public string value { get; set; }
    }
}
