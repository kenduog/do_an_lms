using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.AuthEntities
{
    public class tbl_UserPermission : DomainEntities.DomainEntities
    {
        public Guid? userId { get; set; }
        public Guid? permissionId { get; set; }
    }
}
