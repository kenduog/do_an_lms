using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class RoleUpdate
    {
        /// <summary>
        /// Admin = 1,
        /// Teacher = 2,
        /// Parents = 3,
        /// Manager = 4
        public Group code { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
    }
}
