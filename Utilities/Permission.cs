using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Permission
    {
        public string controller { get; set; }
        public string action { get; set; }
        /// <summary>
        /// true - Có quyền
        /// </summary>
        public bool grant { get; set; }
    }
}
