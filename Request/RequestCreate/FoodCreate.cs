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
    public class FoodCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string description { get; set; }
        public List<FoodItemCreate> items { get; set; }
    }
}
