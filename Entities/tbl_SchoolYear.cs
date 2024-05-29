using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_SchoolYear : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Hiệu trưởng
        /// </summary>
        public Guid? principalId { get; set; }
        public string name { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        [NotMapped]
        public string principalName { get; set; }
    }
}
