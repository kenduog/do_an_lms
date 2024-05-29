using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FeedbackGroup : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        /// <summary>
        /// Tên nhóm phản hồi
        /// </summary>
        public string name_vi { get; set; }
        public string name_en { get; set; }
    }
}
