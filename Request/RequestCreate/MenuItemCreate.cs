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
    public class MenuItemCreate : DomainCreate
    {
        public Guid? itemId { get; set; }
        public double? qty { get; set; }
        public Guid? unitOfMeasureId { get; set; }
    }
}
