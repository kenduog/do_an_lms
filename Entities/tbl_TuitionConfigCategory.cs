using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entities
{
    public class tbl_TuitionConfigCategory : DomainEntities.DomainEntities
    {
        public string name { get; set; }
    }
}
