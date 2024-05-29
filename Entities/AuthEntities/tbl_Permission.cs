using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.AuthEntities
{
    public class tbl_Permission : DomainEntities.DomainEntities
    {
        public Guid? contentTypeId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        /// <summary>
        /// API to call
        /// </summary>
        public string actions { get; set; }
        public string entity { get; set; }

        public bool isParent { get; set; } = false;
        [NotMapped]
        public bool? allowed { get; set; }
        [NotMapped]
        public Guid? groupPermissionId { get; set; }
    }
}
