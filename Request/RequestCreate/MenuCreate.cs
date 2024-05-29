using Entities;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class MenuCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }
        [Required (ErrorMessage = MessageContants.req_nutritionGroup)]
        public Guid? nutritionGroupId { get; set; }
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        [Required (ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        public List<MenuItemCreate> items { get; set; }
        public List<MenuFoodCreate> foods { get; set; }
    }
}
