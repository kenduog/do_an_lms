using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_GroupNews : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Tên nhóm
        /// </summary>
        public string name { get; set; }
        public string background { get; set; }

        [NotMapped]
        public int countMember { get; set; }
    }
}
