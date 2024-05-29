using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_Role : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public string code { get; set; }
        public string permissions { get; set; }
    }

    public class RoleModel
    {
        public string name { get; set; }
        public string code { get; set; }
    }
}
