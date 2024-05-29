using Newtonsoft.Json;
using Request.DomainRequests;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class MenuUpdate : DomainUpdate
    {
        public Guid? gradeId { get; set; }
        public Guid? nutritionGroupId { get; set; }
        public string name { get; set; }
        public List<MenuItemUpdate> items { get; set; }
        public List<MenuFoodUpdate> foods { get; set; }
    }
}
