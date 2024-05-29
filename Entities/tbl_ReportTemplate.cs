using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ReportTemplate : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public string tokens { get; set; }
    }
}
