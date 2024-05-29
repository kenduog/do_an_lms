using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.Auth
{
    public class AccountModel
    {
        public Guid id { get; set; }
        public string fullName { get; set; }
        public string roleCode { get; set; }
        public string roleName { get; set; }
        public bool? isSuperUser { get; set; }
    }
}
