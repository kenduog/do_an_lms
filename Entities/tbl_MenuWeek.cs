using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_MenuWeek : DomainEntities.DomainEntities
    {
        public Guid? weekId { get; set; }
        public Guid? menuId { get; set; }
        /// <summary>
        /// Ngày trong tuần: 1 (CN) -> 7 (Saturday)
        /// </summary>
        public int? day { get; set; }
        [NotMapped]
        public virtual List<FoodItem> foods { get; set; }
    }
}
