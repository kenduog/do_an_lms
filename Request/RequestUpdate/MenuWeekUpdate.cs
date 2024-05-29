using Request.DomainRequests;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class MenuWeekUpdate
    {
        [Required(ErrorMessage = MessageContants.req_weekId)]
        public Guid? weekId { get; set; }
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid branchId { get; set; }
        public List<MenuWeekItemUpdate> items { get; set; }
    }
    public class MenuWeekItemUpdate : DomainUpdate
    {
        /// <summary>
        /// Ngày trong tuần: 1 (CN) -> 7 (Saturday)
        /// </summary>
        public int? day { get; set; }
        public Guid? menuId { get; set; }
    }
}
