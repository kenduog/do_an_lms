using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_AutoGenCodeConfig : DomainEntities.DomainEntities
    {
        public string tableName { get; set; }
        public int? type { get; set; }
        public string prefix { get; set; }
        public bool? isYear { get; set; }
        public bool? isMonth { get; set; }
        public bool? isDay { get; set; }
        public int? autoNumberLength { get; set; }
        public string description { get; set; }
        public int? currentCode { get; set; }
        public string numberIsMissed { get; set; }
        public string note { get; set; }
        public int? currentNumber { get; set; }
    }
}
