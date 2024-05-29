using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Template : DomainEntities.DomainEntities
    {
        /// <summary>
        /// 1 - Phiếu thu
        /// 2 - Phiếu chi
        /// </summary>
        public int type { get; set; }
        public string typeName { get; set; }
        public string content { get; set; }
        public static string GetTypeName(int type) => type == 1 ? "Phiếu thu" : type == 2 ? "Phiếu chi" : "";
    }
}
