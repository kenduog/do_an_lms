using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Profile : DomainEntities.DomainEntities
    {
        public Guid? profileTemplateId { get; set; }
        [NotMapped]
        public string name { get; set; }
        /// <summary>
        /// 1 - Chữ - Text
        /// 2 - Đúng/Sai - Option
        /// </summary>
        [NotMapped]
        public int type { get; set; }
        public Guid? studentId { get; set; }
        public string text { get; set; }
        public bool? option { get; set; }
    }
}
