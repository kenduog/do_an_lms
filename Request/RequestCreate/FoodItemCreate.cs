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
    public class FoodItemCreate: DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_itemId)]
        public Guid? itemId { get; set; }
        public double? qty { get; set; } = 1;
        public double? essenceRate { get; set; }
        public double? weightPerUnit { get; set; }
        public double? actualQty { get; set; }
        public double? calo { get; set; }
        public double? lipit { get; set; }
        public double? protein { get; set; }
        public double? gluxit { get; set; }
    }
}
