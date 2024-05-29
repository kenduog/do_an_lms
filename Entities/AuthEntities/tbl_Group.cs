using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.AuthEntities
{
    public class tbl_Group : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public string code { get; set; }
        public bool? userCanEdit { get; set; } = true;

        [NotMapped]
        public string createdByName { get; set; }
        [NotMapped]
        public string updatedByName { get; set; }
    }

    public class RoleModel
    {
        public string name { get; set; }
        public string code { get; set; }
    }
}
