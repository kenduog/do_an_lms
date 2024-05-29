using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class MenuFoodUpdate : DomainUpdate
    {
        public Guid? foodId { get; set; }
        public int? type { get; set; }
    }
}
