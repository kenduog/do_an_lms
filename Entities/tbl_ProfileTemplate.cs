using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ProfileTemplate: DomainEntities.DomainEntities
    {
        public string name { get; set; }
        /// <summary>
        /// 1 - Chữ - Text
        /// 2 - Đúng/Sai - Option
        /// </summary>
        public int? type { get; set; }
        public int? index { get; set; }
    }
}
