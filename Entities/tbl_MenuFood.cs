using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_MenuFood : DomainEntities.DomainEntities
    {
        public Guid? menuId { get; set; }
        public Guid? foodId { get; set; }
        [NotMapped]
        public string foodName { get; set; }
        /// <summary>
        /// Buổi trong ngày
        /// 1 - Trưa
        /// 2 - Chiều
        /// 3 - Phụ chiều
        /// 4 - Sáng
        /// </summary>
        public int? type { get; set; }
    }
}
