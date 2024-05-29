

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_UnitOfMeasure : DomainEntities.DomainEntities
    {
        public string name { get; set; }
    }
}
