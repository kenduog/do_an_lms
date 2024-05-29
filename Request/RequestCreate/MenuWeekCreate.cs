using Entities;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class MenuWeekCreate : DomainCreate
    {
        public Guid? weekId { get; set; }
        public List<MenuWeekItem> items { get; set; }
    }

    public class MenuWeekItem
    {
        public Guid? menuId { get; set; }
        /// <summary>
        /// Ngày trong tuần: 1 (CN) -> 7 (Saturday)
        /// </summary>
        public int? day { get; set; }
        public List<FoodItem> foods { get; set; }
    }
}
